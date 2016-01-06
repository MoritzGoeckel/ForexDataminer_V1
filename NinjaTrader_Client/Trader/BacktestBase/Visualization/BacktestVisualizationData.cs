using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NinjaTrader_Client.Trader.BacktestBase.Visualization
{
    public class BacktestVisualizationData
    {
        public Dictionary<string, BacktestVisualizationDataComponent> components;

        public BacktestVisualizationData()
        {
            components = new Dictionary<string, BacktestVisualizationDataComponent>();
        }

        public BacktestVisualizationDataComponent addComponent(BacktestVisualizationDataComponent component)
        {
            if (components.ContainsKey(component.getName()) == false)
            {
                components.Add(component.getName(), component);
                return component;
            }
            throw new Exception("Component allready exists");
        }

        public BacktestVisualizationData Copy()
        {
            BacktestVisualizationData copy = new BacktestVisualizationData();
            copy.components = new Dictionary<string, BacktestVisualizationDataComponent>();

            foreach(KeyValuePair<string, BacktestVisualizationDataComponent> component in components)
                copy.components.Add(component.Key, component.Value.Copy());

            return copy;
        }
    }

    public class BacktestVisualizationDataComponent
    {
        public enum VisualizationType { OnChart, ZeroToOne, OneToMinusOne, TrafficLight };

        private string name;
        public VisualizationType type;
        public double value;
        public int chartId;

        public BacktestVisualizationDataComponent(string name, VisualizationType type, int chartId, double value = double.NaN)
        {
            this.name = name;
            this.type = type;
            this.value = value;
            this.chartId = chartId;
        }

        public string getName()
        {
            return name;
        }

        public BacktestVisualizationDataComponent Copy()
        {
            return new BacktestVisualizationDataComponent(name, type, chartId, value);
        }
    }
}
