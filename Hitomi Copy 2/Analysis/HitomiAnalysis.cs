﻿/* Copyright (C) 2018. Hitomi Parser Developers */

using Hitomi_Copy.Data;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Hitomi_Copy_2.Analysis
{
    public class HitomiAnalysis
    {
        private static readonly Lazy<HitomiAnalysis> instance = new Lazy<HitomiAnalysis>(() => new HitomiAnalysis());
        public static HitomiAnalysis Instance => instance.Value;

        List<HitomiAnalysisArtist> datas = new List<HitomiAnalysisArtist>();

        public List<Tuple<string, double, string>> Rank;

        public HitomiAnalysis()
        {
            Dictionary<string, List<HitomiMetadata>> artists = new Dictionary<string, List<HitomiMetadata>>();
            foreach (var metadata in HitomiData.Instance.metadata_collection)
                if (metadata.Artists != null)
                    foreach (var artist in metadata.Artists)
                        if (artists.ContainsKey(artist))
                            artists[artist].Add(metadata);
                        else
                            artists.Add(artist, new List<HitomiMetadata>() { metadata });

            foreach (var pair in artists)
                datas.Add(new HitomiAnalysisArtist(pair.Key, pair.Value));

            Update();
        }

        public void Update()
        {
            HitomiAnalysisArtist user;
            user = new HitomiAnalysisArtist(HitomiLog.Instance.GetEnumerator());
            
            ///////////////////////////////

            Dictionary<string, Tuple<double, HitomiAnalysisArtist>> score = new Dictionary<string, Tuple<double, HitomiAnalysisArtist>>();
            foreach (var pair in user.GetDictionary())
            {
                foreach(var data in datas)
                {
                    if (data.IsExsit(pair.Key))
                        if (score.ContainsKey(data.Aritst))
                            score[data.Aritst] = new Tuple<double, HitomiAnalysisArtist> (score[data.Aritst].Item1+pair.Value * data.GetRate(pair.Key), score[data.Aritst].Item2);
                        else
                            score.Add(data.Aritst, new Tuple<double, HitomiAnalysisArtist>(pair.Value * data.GetRate(pair.Key), data));
                }
            }

            var list = score.ToList();
            list.Sort((p1, p2) => p2.Value.Item1.CompareTo(p1.Value.Item1));

            if (Rank != null) Rank.Clear();
            else Rank = new List<Tuple<string, double, string>>();

            foreach (var item in list)
            {
                Rank.Add(new Tuple<string, double, string>(item.Key, item.Value.Item1, item.Value.Item2.GetDetail(user)));
            }
        }
    }
}
