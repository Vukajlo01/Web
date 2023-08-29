
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;

class CRUD
{
    public static void AddInstance<T>(T instance, string filePath, string[] defaultInstance = null)
    {
        PropertyInfo[] properties = typeof(T).GetProperties();

        int i = -1;
        string data = "";
        foreach (PropertyInfo property in properties)
        {
            i++;
            if (property.GetValue(instance) == null && defaultInstance != null)
            {
                try
                {
                    data += defaultInstance[i] + ";";
                } catch
                {
                    data += ';';
                }
                continue;
            }

            if (property.GetValue(instance) is HashSet<Guid>)
            {
                System.Diagnostics.Debug.WriteLine(property.GetValue(instance));
                System.Diagnostics.Debug.WriteLine(property.Name);
                foreach(var item in (HashSet<Guid>)property.GetValue(instance))
                {
                    data += item;
                    data += ",";
                }
                data += ";";
                continue;
            }

            if (property.GetValue(instance) is DateTime)
            {
                DateTime time = (DateTime)property.GetValue(instance);
                data += time.ToString("yyyy-MM-dd") + ";";
                continue;
            }

            data += property.GetValue(instance);
            data += ";";
        }

        using (var writer = new StreamWriter(filePath, true))
        {
            writer.WriteLine(data);
        }
    }

    public static void UpdateInstance<T>(String field, int index, T new_instance, string filePath)
    {
        var lines = File.ReadAllLines(filePath).ToList();

        int i = -1;
        bool flag = false;
        string[] defaultInstance = null;
        foreach (var line in lines)
        {
            i++;
            if (line.Split(';')[index] == field)
            {
                defaultInstance = line.Split(';');
                flag = true;
                break;
            }
        }

        if (!flag)
            return;

        lines.RemoveAt(i);

        using (var writer = new StreamWriter(filePath, false))
        {
            foreach (var line in lines)
            {
                writer.WriteLine(line);
            }
        }

        AddInstance(new_instance, filePath, defaultInstance);
    }

    public static List<string> RetrieveInstance(string field, int index, string filePath)
    {
        var lines = File.ReadAllLines(filePath);

        var existingInstance = lines.FirstOrDefault(line =>
        {
            var d = line.Split(';');
            return d[index] == field;
        });

        if (existingInstance == null)
        {
            return null;
        }

        return existingInstance.Split(';').ToList();
    }
}