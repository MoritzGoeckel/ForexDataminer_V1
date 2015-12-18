using NinjaTrader_Client.Trader.MainAPIs;
using NinjaTrader_Client.Trader.Model;
using System;
using System.Collections.Generic;

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
            public List<TimeValueData> timeValueData;
            public List<Tickdata> tickData;
        }

        Dictionary<string, IndicatorCache> caches = new Dictionary<string, IndicatorCache>();

        public override TimeValueData getIndicator(long timestamp, string instrument)
        {
            if (caches.ContainsKey(instrument) == false)
                caches.Add(instrument, new IndicatorCache());

            IndicatorCache cache = caches[instrument];

            double removedMin = double.MaxValue, removedMax = double.MinValue, min = double.MaxValue, max = double.MinValue;

            List<Tickdata> data;

            //Es gibt überschneidungen
            if (timestamp - timeframe < cache.timestamp && timestamp > cache.timestamp && cache.timestamp - timeframe < timestamp - timeframe && cache.timestamp != 0)
            {
                List<Tickdata> newData = database.getPrices(cache.timestamp, timestamp, instrument);

                data = cache.tickData;
                data.AddRange(newData);

                cache.timestamp = timestamp;

                //remove the first element until in timeframe
                while (data.Count != 0)
                {
                    if (data[0].timestamp >= timestamp - timeframe)
                        break;
                    else
                    {
                        if (data[0].getAvg() > removedMax)
                            removedMax = data[0].getAvg();

                        if (data[0].getAvg() < removedMin)
                            removedMin = data[0].getAvg();

                        data.RemoveAt(0);
                    }
                }

                //Suche neues min max, wenn removed worden
                if (cache.min == removedMin || cache.max == removedMax)
                {
                    //Suche komplett

                    if (data.Count > 0)
                    {
                        getMinMaxInPrices(ref min, ref max, data);
                        cache.min = min;
                        cache.max = max;
                    }
                    else
                    {
                        min = cache.min;
                        max = cache.max;
                    }
                }
                else //Suche sonst nur im newData
                {
                    if (newData.Count > 0) //Nur wenn New Data daten hat
                    {
                        //Suche nur in neuem
                        getMinMaxInPrices(ref min, ref max, newData);

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
                data = database.getPrices(timestamp - timeframe, timestamp, instrument);
                getMinMaxInPrices(ref min, ref max, data);

                cache.tickData = data;
                cache.min = min;
                cache.max = max;
                cache.timestamp = timestamp;
            }

            if (data.Count != 0)
            {
                Tickdata lastTick = data[data.Count - 1];
                double now = lastTick.getAvg();

                return new TimeValueData(lastTick.timestamp, (now - min) / (max - min));
            }
            return new TimeValueData(timestamp, 0.5);
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

                data = cache.timeValueData;
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

                    if (data.Count > 0)
                    {
                        getMinMaxInData(ref min, ref max, data);
                        cache.min = min;
                        cache.max = max;
                    }
                    else
                    {
                        min = cache.min;
                        max = cache.max;
                    }
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

                cache.timeValueData = data;
                cache.min = min;
                cache.max = max;
                cache.timestamp = timestamp;
            }

            if (data.Count != 0)
            {
                TimeValueData lastTick = data[data.Count - 1];
                double now = lastTick.value;

                return new TimeValueData(lastTick.timestamp, (now - min) / (max - min));
            }
            return new TimeValueData(timestamp, 0.5);
        }

        private void getMinMaxInPrices(ref double min, ref double max, List<Tickdata> data)
        {
            min = double.MaxValue;
            max = double.MinValue;

            foreach (Tickdata tick in data)
            {
                if (tick.getAvg() > max)
                    max = tick.getAvg();

                if (tick.getAvg() < min)
                    min = tick.getAvg();
            }

            if (data.Count == 0 || min == double.MaxValue || max == double.MinValue)
                throw new Exception("StochIndicator getMinMaxInData: data.Count == 0 or min/max not set");
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
                throw new Exception("StochIndicator getMinMaxInData: data.Count == 0 or min/max not set");
        }
    }
}
