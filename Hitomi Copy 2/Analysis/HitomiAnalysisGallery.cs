﻿/* Copyright (C) 2018. Hitomi Parser Developers */

using Hitomi_Copy.Data;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Hitomi_Copy_2.Analysis
{
    public class HitomiAnalysisGallery
    {
        public List<KeyValuePair<int, Tuple<double, HitomiMetadata>>> gallery_data;
        public HitomiAnalysisGallery()
        {
            Dictionary<string, int> tag_rank = new Dictionary<string, int>();
            foreach (var v in HitomiLog.Instance.GetEnumerator())
            {
                if (v.Tags == null) continue;
                foreach (var tag in v.Tags)
                {
                    string legalize = HitomiCommon.LegalizeTag(tag);
                    if (tag_rank.ContainsKey(legalize))
                        tag_rank[legalize] += 1;
                    else
                        tag_rank.Add(legalize, 1);
                }
            }

            Dictionary<int, Tuple<double, HitomiMetadata>> datas = new Dictionary<int, Tuple<double, HitomiMetadata>>();
            double total_score = 0.0;
            int count_metadata = HitomiData.Instance.metadata_collection.Count;
            foreach (var metadata in HitomiData.Instance.metadata_collection)
            {
                double score = 0.0;
                if (metadata.Tags != null)
                {
                    foreach (var tag in metadata.Tags)
                        if (tag_rank.ContainsKey(tag))
                            score += tag_rank[tag];
                    score /= metadata.Tags.Length;
                }
                total_score += score;
                datas.Add(metadata.ID, new Tuple<double, HitomiMetadata>(score, metadata));
            }

            gallery_data = datas.ToList();
            gallery_data.Sort((p1, p2) => p2.Value.Item1.CompareTo(p1.Value.Item1));
        }
    }
}
