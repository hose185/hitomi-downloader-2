﻿/* Copyright (C) 2018. Hitomi Parser Developers */

using Newtonsoft.Json;
using System;
using System.IO;

namespace Hitomi_Copy_2
{
    public class HitomiSettingModel
    {
        [JsonProperty]
        public string Path;
        [JsonProperty]
        public string[] ExclusiveTag;
        [JsonProperty]
        public bool Zip;
        [JsonProperty]
        public int MaximumThumbnailShow;
        [JsonProperty]
        public int Thread;
    }

    public class HitomiSetting
    {
        private static readonly Lazy<HitomiSetting> instance = new Lazy<HitomiSetting>(() => new HitomiSetting());
        public static HitomiSetting Instance => instance.Value;
        string log_path = $"{Environment.CurrentDirectory}\\setting.json";

        HitomiSettingModel model;

        public HitomiSetting()
        {
            if (File.Exists(log_path)) model = JsonConvert.DeserializeObject<HitomiSettingModel>(File.ReadAllText(log_path));
            if (model == null)
            {
                model = new HitomiSettingModel();
                model.Path = @"C:\Hitomi\{Artists}\[{Id}] {Title}\";
                model.ExclusiveTag = new string[] { "female:mother", "male:anal", "male:guro", "female:guro", "male:snuff", "female:snuff" };
                model.Zip = false;
                model.MaximumThumbnailShow = 1000;
                model.Thread = 32;
                Save();
            }
            else
            {
                if (String.IsNullOrEmpty(model.Path)) model.Path = @"C:\Hitomi\{Artists}\[{Id}] {Title}\";
                if (model.MaximumThumbnailShow < 10) model.MaximumThumbnailShow = 1000;
                if (model.Thread < 5) model.Thread = 32;
                Save();
            }
        }

        public void Save()
        {
            string json = JsonConvert.SerializeObject(model, Formatting.Indented);
            using (var fs = new StreamWriter(new FileStream(log_path, FileMode.Create, FileAccess.Write)))
            {
                fs.Write(json);
            }
        }
        
        public ref HitomiSettingModel GetModel()
        {
            return ref model;
        }
        
    }
}
