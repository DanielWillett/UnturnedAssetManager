using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace UAM;

#nullable enable

public struct AssetFile
{
    public System.IO.FileInfo file;
    public System.IO.FileInfo? local;
    public Guid guid;
    public ushort ID;
    public EAssetType[] types;
    public string Name;
    public EAssetCategory category;
    public StringPair[] pairs;
    public StringPair[]? localPairs;
    public bool initSuccess = false;
    public AssetFile(System.IO.FileInfo path, System.IO.FileInfo? englishPath = null)
    {
        file = path;
        local = englishPath;
        localPairs = null;
        Name = null!;
        guid = Guid.Empty;
        ID = 0;
        category = EAssetCategory.NONE;
        if (!file.Exists)
        {
            Debug.WriteLine("Failed to find file " + file.FullName);
            types = new EAssetType[0];
            pairs = new StringPair[0];
        }

        using (FileStream stream = new FileStream(file.FullName, FileMode.Open, FileAccess.Read, FileShare.Read))
        {
            byte[] buffer = new byte[stream.Length];
            stream.Read(buffer, 0, buffer.Length);
            string text = System.Text.Encoding.UTF8.GetString(buffer);

            string[] lines = text.Split('\n');
            if (lines.Length != 0)
            {
                List<string> lines2 = new List<string>(lines.Length);
                for (int i = 0; i < lines.Length; i++)
                {
                    if (!string.IsNullOrEmpty(lines[i]))
                        lines2.Add(lines[i].Replace("\r", string.Empty));
                }
                pairs = new StringPair[lines2.Count];
                for (int i = 0; i < lines2.Count; i++)
                {
                    pairs[i] = new StringPair(lines2[i]);
                }
            }
            else
            {
                pairs = new StringPair[0];
            }
        }

        types = null!;
        bool fguid = false;
        bool fid = false;
        
        for (int i = 0; i < pairs.Length; i++)
        {
            if (pairs[i].key == "Type")
            {
                if (!pairs[i].HasValue) break;
                types = Assets.GetTypes(pairs[i].value, out category);
            }
            else if (pairs[i].key == "GUID")
            {
                if (!pairs[i].HasValue || !Guid.TryParse(pairs[i].value, out guid)) break;
                fguid = true;
            }
            else if (pairs[i].key == "ID")
            {
                if (!pairs[i].HasValue || !ushort.TryParse(pairs[i].value, out ID)) break;
                fid = true;
            }
        }
        if (types == null)
        {
            types = new EAssetType[0];
            Debug.WriteLine("Failed to find type for file " + file.FullName);
            return;
        }
        if (!fguid)
        {
            Debug.WriteLine("Failed to find GUID for file " + file.FullName);
            return;
        }
        if (!fid)
        {
            Debug.WriteLine("Failed to find ID for file " + file.FullName);
            return;
        }


        Name = file.Name.Length > 3 ? file.Name.Substring(0, file.Name.Length - 4) : ID.ToString();

        initSuccess = true;

        if (local == null || !local.Exists)
        {
            localPairs = null;
            return;
        }
        using (FileStream stream = new FileStream(local.FullName, FileMode.Open, FileAccess.Read, FileShare.Read))
        {
            byte[] buffer = new byte[stream.Length];
            stream.Read(buffer, 0, buffer.Length);
            string text = System.Text.Encoding.UTF8.GetString(buffer);

            string[] lines = text.Split('\n');
            if (lines.Length != 0)
            {
                List<string> lines2 = new List<string>(lines.Length);
                for (int i = 0; i < lines.Length; i++)
                {
                    if (!string.IsNullOrEmpty(lines[i]))
                        lines2.Add(lines[i]);
                }
                localPairs = new StringPair[lines2.Count];
                for (int i = 0; i < lines2.Count; i++)
                {
                    localPairs[i] = new StringPair(lines2[i]);
                }
            }
            else
            {
                localPairs = new StringPair[0];
            }
        }
    }
    public string? GetLocalName() => GetLocalProperty("Name");
    public string? GetLocalDescription() => GetLocalProperty("Description");
    public string? GetLocalProperty(string property)
    {
        if (!initSuccess)
            throw new ObjectDisposedException("This asset file failed to initialize.");
        if (property == null)
            throw new ArgumentNullException("Property was null", nameof(property));
        if (localPairs != null)
        {
            for (int i = 0; i < localPairs.Length; i++)
            {
                StringPair val = localPairs[i];
                if (val.HasValue && val.key == property)
                {
                    return val.value;
                }
            }
        }
        return null;
    }
    public string? GetProperty(string property)
    {
        if (!initSuccess)
            throw new ObjectDisposedException("This asset file failed to initialize.");
        if (property == null)
            throw new ArgumentNullException("Property was null", nameof(property));
        for (int i = 0; i < pairs.Length; i++)
        {
            StringPair val = pairs[i];
            if (val.HasValue && val.key == property)
            {
                return val.value;
            }
        }
        return null;
    }
    public bool TryGetProperty(string property, out string value)
    {
        if (!initSuccess)
            throw new ObjectDisposedException("This asset file failed to initialize.");
        if (property == null)
            throw new ArgumentNullException("Property was null", nameof(property));
        for (int i = 0; i < pairs.Length; i++)
        {
            StringPair val = pairs[i];
            if (val.HasValue && val.key == property)
            {
                value = val.value!;
                return true;
            }
        }
        value = null!;
        return false;
    }
    public bool TryGetEnumType<TEnum>(string property, out TEnum @enum) where TEnum : struct
    {
        string? val = GetProperty(property);
        if (val != null)
        {
            return Enum.TryParse(val, false, out @enum);
        }
        @enum = default;
        return false;
    }
    public bool TryGetFloatType(string property, out float @float)
    {
        string? val = GetProperty(property);
        if (val != null)
        {
            return float.TryParse(val, out @float);
        }
        @float = default;
        return false;
    }
    public bool TryGetIntegerType(string property, out int @int)
    {
        string? val = GetProperty(property);
        if (val != null)
        {
            return int.TryParse(val, out @int);
        }
        @int = default;
        return false;
    }
    public bool HasProperty(string property)
    {
        if (!initSuccess)
            throw new ObjectDisposedException("This asset file failed to initialize.");
        if (property == null)
            throw new ArgumentNullException("Property was null", nameof(property));
        for (int i = 0; i < pairs.Length; i++)
            if (pairs[i].key == property)
                return true;
        return false;
    }
}

public struct StringPair
{
    public string key;
    public string? value;
    public bool HasValue => value != null;
    public StringPair(string key, string? value = null)
    {
        this.key = key;
        this.value = value;
    }
    public StringPair(string line)
    {
        if (line == null) throw new ArgumentException("Line is null", nameof(line));
        string[] parts = line.Split(new char[1] { ' ' }, 2);
        if (parts.Length == 0) throw new ArgumentException("Line has no text", nameof(line));
        key = parts[0];
        if (parts.Length > 1)
            value = parts[1];
        else 
            value = null;
    }
}