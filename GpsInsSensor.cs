/**
 * Copyright (c) 2018 LG Electronics, Inc.
 *
 * This software contains code licensed as described in LICENSE.
 *
 */

using Simulator.Bridge;
using Simulator.Bridge.Data;
using Simulator.Utilities;
using UnityEngine;
using Simulator.Sensors.UI;
using System.Collections.Generic;
using System.Collections;
using Simulator.Analysis;

namespace Simulator.Sensors
{
    [SensorType("GPS-INS Status", new[] { typeof(GpsInsData) })]
    public class GpsInsSensor : SensorBase
    {
        [SensorParameter]
        [Range(1.0f, 100f)]
        public float Frequency = 12.5f;

        double NextSend;
        uint SendSequence;

        BridgeInstance Bridge;
        Publisher<GpsInsData> Publish;
        
        public override SensorDistributionType DistributionType => SensorDistributionType.MainOrClient;
        public override float PerformanceLoad { get; } = 0.05f;

        public override void OnBridgeSetup(BridgeInstance bridge)
        {
            Bridge = bridge;
            Publish = Bridge.AddPublisher<GpsInsData>(Topic);
        }

        protected override void Initialize()
        {
            NextSend = SimulatorManager.Instance.CurrentTime + 1.0f / Frequency;
        }

        protected override void Deinitialize()
        {
            
        }

        void Update()
        {
            if (Bridge == null || Bridge.Status != Status.Connected)
            {
                return;
            }

            if (SimulatorManager.Instance.CurrentTime < NextSend)
            {
                return;
            }
            NextSend = SimulatorManager.Instance.CurrentTime + 1.0f / Frequency;
            
            Publish(new GpsInsData()
            {
                Name = Name,
                Frame = Frame,
                Time = SimulatorManager.Instance.CurrentTime,
                Sequence = SendSequence++,

                Status = 3,
                PositionType = 56,
            });
        }

        public override void OnVisualize(Visualizer visualizer)
        {
            Debug.Assert(visualizer != null);

            var graphData = new Dictionary<string, object>()
            {
                {"Status", 3},
                {"Position Type", 56}
            };
            visualizer.UpdateGraphValues(graphData);
        }

        public override void SetAnalysisData()
        {
            SensorAnalysisData = new List<AnalysisReportItem>
            {
                new AnalysisReportItem
                {
                    name = "Status",
                    type = MeasurementType.Misc,
                    value = 3
                },
                new AnalysisReportItem
                {
                    name = "Position Type",
                    type = MeasurementType.Misc,
                    value = 56
                },
            };
        }

        public override void OnVisualizeToggle(bool state)
        {
            //
        }
    }
}
