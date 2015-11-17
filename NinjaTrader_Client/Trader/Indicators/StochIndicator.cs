﻿using NinjaTrader_Client.Trader.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NinjaTrader_Client.Trader.Indicators
{
    class StochIndicator : Indicator
    {
        private int timeframe;

        public StochIndicator(Database database, int timeframe) : base(database)
        {
            this.timeframe = timeframe;
        }

        private class IndicatorCache
        {
            public long timestamp = 0;
            public double min, max;
            public List<TimeValueData> data;
        }

        Dictionary<string, IndicatorCache> caches = new Dictionary<string, IndicatorCache>();

        //Todo: Caching, Abstract
        //Maybe just save the data to save cpu?
        public override TimeValueData getIndicator(long timestamp, string instrument) //Noch nicht optimiert. Siehe anderen getIndicator!
        {
            List<Tickdata> data = database.getPrices(timestamp - timeframe, timestamp, instrument); //Flaschenhals ???

            if (data.Count == 0)
                return null;

            double min = double.MaxValue;
            double max = double.MinValue;

            foreach(Tickdata tick in data)
            {
                if (tick.ask > max)
                    max = tick.ask;

                if (tick.bid < min)
                    min = tick.bid;
            }

            Tickdata lastTick = data[data.Count - 1];

            double now = lastTick.ask;

            return new TimeValueData(lastTick.timestamp, (now - min) / (max - min));
        }

        public override TimeValueData getIndicator(long timestamp, string dataName, string instrument)
        {
            if (caches.ContainsKey(dataName + instrument) == false)
                caches.Add(dataName + instrument, new IndicatorCache());

            IndicatorCache cache = caches[dataName + instrument];

            double removedMin = double.MaxValue, removedMax = double.MinValue, min = double.MaxValue, max = double.MinValue;

            List<TimeValueData> data;

            //Es gibt überschneidungen
            if (timestamp - timeframe < cache.timestamp && timestamp > cache.timestamp && cache.timestamp - timeframe < timestamp - timeframe && cache.timestamp != 0)
            {
                List<TimeValueData> newData = database.getDataInRange(cache.timestamp, timestamp, dataName, instrument);

                data = cache.data;
                data.AddRange(newData);

                cache.timestamp = timestamp;

                //remove the first element until in timeframe
                while (data.Count != 0)
                {
                    if (data[0].timestamp >= timestamp - timeframe)
                        break;
                    else
                    {
                        if (data[0].value > removedMax)
                            removedMax = data[0].value;

                        if (data[0].value < removedMin)
                            removedMin = data[0].value;

                        data.RemoveAt(0);
                    }
                }

                //Suche neues min max, wenn removed worden
                if (cache.min == removedMin || cache.max == removedMax)
                {
                    //Suche komplett
                    getMinMaxInData(ref min, ref max, data);
                    cache.min = min;
                    cache.max = max;
                }
                else //Suche sonst nur im newData
                {
                    if (newData.Count > 0) //Nur wenn New Data daten hat
                    {
                        //Suche nur in neuem
                        getMinMaxInData(ref min, ref max, newData);

                        if (min < cache.min)
                            cache.min = min;
                        else
                            min = cache.min;

                        if (max > cache.max)
                            cache.max = max;
                        else
                            max = cache.max;
                    }
                    else //sonst nehme das alte
                    {
                        min = cache.min;
                        max = cache.max;
                    }
                }
            }
            else //Es gibt keine überschneidungen
            {
                data = database.getDataInRange(timestamp - timeframe, timestamp, dataName, instrument);
                getMinMaxInData(ref min, ref max, data);

                cache.data = data;
                cache.min = min;
                cache.max = max;
                cache.timestamp = timestamp;
            }

            TimeValueData lastTick = data[data.Count - 1];
            double now = lastTick.value;

            return new TimeValueData(lastTick.timestamp, (now - min) / (max - min));
        }

        private void getMinMaxInData(ref double min, ref double max, List<TimeValueData> data)
        {
            min = double.MaxValue;
            max = double.MinValue;

            foreach (TimeValueData tick in data)
            {
                if (tick.value > max)
                    max = tick.value;

                if (tick.value < min)
                    min = tick.value;
            }

            if (data.Count == 0 || min == double.MaxValue || max == double.MinValue)
                throw new Exception();
        }
    }
}
