using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;

namespace UAM;

#nullable enable

public struct AssetFile
{
    public static CultureInfo info = new CultureInfo("en-US");
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
    private static readonly List<string> linesList = new List<string>(32);
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
                linesList.Clear();
                for (int i = 0; i < lines.Length; i++)
                {
                    if (!string.IsNullOrEmpty(lines[i]))
                        linesList.Add(lines[i].Replace("\r", string.Empty));
                }
                pairs = new StringPair[linesList.Count];
                for (int i = 0; i < linesList.Count; i++)
                {
                    pairs[i] = new StringPair(linesList[i]);
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
                linesList.Clear();
                for (int i = 0; i < lines.Length; i++)
                {
                    if (!string.IsNullOrEmpty(lines[i]))
                        linesList.Add(lines[i]);
                }
                localPairs = new StringPair[linesList.Count];
                for (int i = 0; i < linesList.Count; i++)
                {
                    localPairs[i] = new StringPair(linesList[i]);
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
    private int index = 0;
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
                if (val.key == property)
                {
                    if (val.HasValue)
                        return val.value!.Replace('\r', '\0');
                    return null;
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
        if (index >= pairs.Length) index = pairs.Length - 1;
        int startingIndex = index;
        int i = index;
        for (; i < pairs.Length; i++)
        {
            StringPair val = pairs[i];
            if (val.key == property)
            {
                index = (i < pairs.Length - 1) ? i + 1 : 0;
                if (val.HasValue)
                    return val.value!.Replace('\r', '\0');
                return null;
            }
        }
        i = 0;
        for (; i < startingIndex; i++)
        {
            StringPair val = pairs[i];
            if (val.key == property)
            {
                index = (i < pairs.Length - 1) ? i + 1 : 0;
                if (val.HasValue)
                    return val.value!.Replace('\r', '\0');
                return null;
            }
        }

        index = i;
        return null;
    }
    public string GetProperty(string property, string @default)
    {
        if (!initSuccess)
            throw new ObjectDisposedException("This asset file failed to initialize.");
        if (property == null)
            throw new ArgumentNullException("Property was null", nameof(property));
        for (int i = 0; i < pairs.Length; i++)
        {
            StringPair val = pairs[i];
            if (val.key == property)
            {
                if (val.HasValue)
                    return val.value!.Replace('\r', '\0');
                return @default;
            }
        }
        return @default;
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
            return Enum.TryParse(val, true, out @enum);
        }
        @enum = default;
        return false;
    }
    public bool TryGetFloatType(string property, out float @float)
    {
        string? val = GetProperty(property);
        if (val != null)
        {
            return float.TryParse(val, NumberStyles.Any, info, out @float);
        }
        @float = default;
        return false;
    }
    public bool TryGetIntegerType(string property, out int @int)
    {
        string? val = GetProperty(property);
        if (val != null)
        {
            return int.TryParse(val, NumberStyles.Any, info, out @int);
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
    public TEnum GetEnumType<TEnum>(string property, TEnum @default = default) where TEnum : struct
    {
        string? val = GetProperty(property);
        if (val != null)
        {
            return Enum.TryParse(val, true, out TEnum @enum) ? @enum : @default;
        }
        else return @default;
    }
    public float GetFloatType(string property, float @default = default)
    {
        string? val = GetProperty(property);
        if (val != null)
        {
            return float.TryParse(val, NumberStyles.Any, info, out float @float) ? @float : @default;
        }
        else return @default;
    }
    public Guid GetGUIDType(string property, Guid @default = default)
    {
        string? val = GetProperty(property);
        if (val != null)
        {
            return val.Length == 0 && val[0] == '0' ? Guid.Empty : (Guid.TryParse(val, out Guid guid) ? guid : @default);
        }
        else return @default;
    }
    public int GetIntegerType(string property, int @default = default)
    {
        string? val = GetProperty(property);
        if (val != null)
        {
            return int.TryParse(val, NumberStyles.Any, info, out int @int) ? @int : @default;
        }
        else return @default;
    }
    public uint GetUIntegerType(string property, uint @default = default)
    {
        string? val = GetProperty(property);
        if (val != null)
        {
            return uint.TryParse(val, NumberStyles.Any, info, out uint @uint) ? @uint : @default;
        }
        else return @default;
    }
    public ushort GetUShortType(string property, ushort @default = default)
    {
        string? val = GetProperty(property);
        if (val != null)
        {
            return ushort.TryParse(val, NumberStyles.Any, info, out ushort @ushort) ? @ushort : @default;
        }
        else return @default;
    }
    public short GetShortType(string property, short @default = default)
    {
        string? val = GetProperty(property);
        if (val != null)
        {
            return short.TryParse(val, NumberStyles.Any, info, out short @uhort) ? @uhort : @default;
        }
        else return @default;
    }
    public byte GetByteType(string property, byte @default = default)
    {
        string? val = GetProperty(property);
        if (val != null)
        {
            return byte.TryParse(val, NumberStyles.Any, info, out byte @byte) ? @byte : @default;
        }
        else return @default;
    }
    public bool GetBooleanType(string property, bool @default = default)
    {
        string? val = GetProperty(property);
        if (val != null)
        {
            return val.Equals("true", StringComparison.InvariantCultureIgnoreCase) || 
                   val == "1" ||
                   val.Equals("y", StringComparison.InvariantCultureIgnoreCase);
        }
        else return @default;
    }
    public int GetIntegerTypeClampTo1(string property) => Math.Max(1, GetIntegerType(property, 1));
    public int GetIntegerTypeClampTo0(string property) => Math.Max(0, GetIntegerType(property, 0));
    public float GetFloatTypeClampTo1(string property) => Math.Max(1f, GetFloatType(property, 1f));
    public float GetFloatTypeClampTo0(string property) => Math.Max(0f, GetFloatType(property, 0f));
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
    public void SetValue(string? val) => value = val;
}