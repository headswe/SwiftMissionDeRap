using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SwiftMissionDeRap
{
    public class TextClass : SqmClass
    {
        private string Data { get; set; }
        public void readClass(string data)
        {
            string classPattern = @"class\s+(\w*)";
            string attributePattern = @"(\w*)(\[?\]?)=(.*);";
            string arrayPattern = @"""[^""]*""|[^,]+";
            var lines = new Queue<string>(data.Split(new string[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries));
            TextClass currentClass = null;
            var line = "";
            var openBraces = 0;
            while (lines.Count > 0)
            {
                line = lines.Dequeue().Trim('\t');
                if (currentClass == null)
                {
                    var matches = Regex.Matches(line, classPattern);
                    if (matches.Count > 0)
                    {
                        if (currentClass == null)
                        {
                            openBraces = 0;
                            currentClass = new TextClass()
                            {
                                Name = matches[0].Groups[1].Value.Trim(),
                                Data = ""
                            };
                            if (line.Contains('{'))
                            {
                                openBraces++;
                                currentClass.Data += line + "\r\n";
                            }
                            else
                            {
                                while (!line.Contains('{'))
                                {
                                    line = lines.Dequeue();
                                    currentClass.Data += line + "\r\n";
                                }
                                openBraces++;
                            }
                        }
                    }
                    matches = Regex.Matches(line, attributePattern);
                    if (matches.Count > 0)
                    {
                        var name = matches[0].Groups[1].Value;
                        string value = matches[0].Groups[3].Value;
                        if(value.StartsWith("\""))
                            Attributes.Add(name, value.Trim('"'));
                        else if (value.StartsWith("{"))
                        {
                            var list = new List<dynamic>();
                            var items = Regex.Matches(value.Trim(new char[] { '{', '}' }), arrayPattern);
                            for (int i = 0; i < items.Count; i++)
                            {
                                var v = items[i].Value;
                                if (v.StartsWith("\""))
                                    list.Add(v.Trim('\"'));
                                else
                                    list.Add(double.Parse(v, CultureInfo.InvariantCulture));
                            }
                            Attributes.Add(name, list);
                        }
                        else
                        {
                            Attributes.Add(name, double.Parse(value, CultureInfo.InvariantCulture));
                        }
                    }
                }
                else
                {
                    if (line.Contains('{'))
                        openBraces++;
                    if (line.Contains('}'))
                        openBraces--;
                    currentClass.Data += line + "\r\n";
                    if (openBraces == 0)
                    {
                        Children.Add(currentClass.Name, currentClass);
                        currentClass = null;
                    }
                }
            }
            foreach(var child in Children)
            {
                var c = child.Value as TextClass;
                c.readClass(c.Data);
            }
        }
    }
}
