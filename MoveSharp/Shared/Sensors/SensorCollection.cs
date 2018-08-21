using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using MoveSharp.Extensions;
using MoveSharp.Geolocation;
using SharpGeo;

namespace MoveSharp.Sensors {
    /// <summary>
    /// Collection of sensors
    /// </summary>
    public class SensorCollection : IEnumerable<Sensor> {
        private List<Sensor> _sensors = new List<Sensor>();
        public event PropertyChangedEventHandler SensorPropertyChanged;
        public event EventHandler<Sensor> Added;
        public event EventHandler<Sensor> Removed;

        /// <summary>
        /// Adds a sensor to the collection
        /// </summary>
        /// <param name="sensor"><see cref="Sensor"/> instance to be added</param>
        /// <returns>true if the sensor has been added, otherwise false</returns>
        public bool Add(Sensor sensor) {
            // update sensor state from settings
            var sensorAlreadyFound = _sensors.FirstOrDefault(s => s.Id == sensor.Id);
            if (sensorAlreadyFound == null) {
            } else if (sensorAlreadyFound.Implementation is SensorDescription) {
                // just a dummy model, re-add it
                if (sensorAlreadyFound.Implementation is BikeSpeedSensorDescription) {
                    // setup saved bike speed sensor properties
                    var bikeSpeedSensorDesc = (BikeSpeedSensorDescription)sensorAlreadyFound.Implementation;
                    var bikeSpeedSensor = sensor.Implementation as IBikeSpeedSensor;
                    if (bikeSpeedSensor != null)
                        bikeSpeedSensor.WheelSize = bikeSpeedSensorDesc.WheelSize;
                }
                Remove(sensorAlreadyFound);
                sensor.IsEnabled = true; // SensorDescription is created by saving sensor's state
            } else {
                // device already in the list
                return false;
            }

            // no entry for this sensor so far
            _sensors.Add(sensor);
            sensor.EnabledChanged += sensor_EnabledChanged;
            UpdatePrimarySensors(sensor);
            if (Added != null)
                Added(this, sensor);

            return true;
        }

        public void Remove(Sensor sensor) {
            _sensors.Remove(sensor);
            sensor.EnabledChanged -= sensor_EnabledChanged;
            UpdatePrimarySensors(sensor);
            if (Removed != null)
                Removed(this, sensor);
        }

        public int Count {
            get { return _sensors.Count; }
        }

        public void Clear() {
            for (var i = Count - 1; i >= 0; i--) {
                _sensors[i].EnabledChanged -= sensor_EnabledChanged;
                _sensors.RemoveAt(i);
            }
        }

        void sensor_EnabledChanged(object sender, EventArgs e) {
            var sensor = sender as Sensor;
            if (sensor == null)
                return;

            // ISensor has been connected or disconnected
            UpdatePrimarySensors(sensor);

            // forward to the subscribers
            SensorPropertyChanged?.Invoke(sender, new PropertyChangedEventArgs(nameof(Sensor.IsEnabled)));
        }

        private void UpdatePrimarySensors(Sensor sensor) {
            // TODO: more sophisticated primary sensor handling
            if (sensor.Implementation is IGps) {
                Gps = (IGps)sensor.Implementation;
            }
            if (sensor.Implementation is Clock) {
                Clock = (Clock)sensor.Implementation;
            }
            if (sensor.Implementation is IHeartRateMonitor) {
                if (HeartRateMonitor == null)
                    HeartRateMonitor = (IHeartRateMonitor)sensor.Implementation;
                else {
                    if (HeartRateMonitor == sensor.Implementation && sensor.IsEnabled == false) {
                        // search for another heart rate monitor
                        var hrMonitor = _sensors.FirstOrDefault(s => s.Implementation is IHeartRateMonitor && s.IsEnabled);
                        if (hrMonitor != null)
                            HeartRateMonitor = hrMonitor.Implementation as IHeartRateMonitor;
                        else
                            HeartRateMonitor = null;
                    } else if (sensor.IsEnabled) {
                        HeartRateMonitor = (IHeartRateMonitor)sensor.Implementation;
                    }
                }
            }
            if (sensor.Implementation is ISpeedSensor) {
                if (SpeedSensor == null && sensor.IsEnabled) {
                    SpeedSensor = (ISpeedSensor)sensor.Implementation;
                } else {
                    if (SpeedSensor == sensor.Implementation && sensor.IsEnabled == false) {
                        // search for another speed sensor, which is enabled
                        var speedSensor = _sensors.FirstOrDefault(s => s.Implementation is ISpeedSensor && s.IsEnabled);
                        if (speedSensor != null)
                            SpeedSensor = speedSensor.Implementation as ISpeedSensor;
                        else
                            SpeedSensor = null;
                    } else if (sensor.IsEnabled) {
                        SpeedSensor = (ISpeedSensor)sensor.Implementation;
                    }
                }
            }
            if (sensor.Implementation is ICadenceSensor) {
                if (CadenceSensor == null && sensor.IsEnabled) {
                    // no cadence sensor has been set yet
                    CadenceSensor = (ICadenceSensor)sensor.Implementation;
                } else {
                    if (CadenceSensor == sensor.Implementation && sensor.IsEnabled == false) {
                        // search for another cadence sensor, which is enabled
                        var cadenceSensor = _sensors.FirstOrDefault(s => s.Implementation is ICadenceSensor && s.IsEnabled);
                        if (cadenceSensor != null)
                            CadenceSensor = cadenceSensor.Implementation as ICadenceSensor;
                        else
                            CadenceSensor = null;
                    } else if (sensor.IsEnabled) {
                        CadenceSensor = (ICadenceSensor)sensor.Implementation;
                    }
                }
            }
            if (sensor.Implementation is IBikePowerMeter) {
                Update(sensor, ref _bikePowerMeter, (IBikePowerMeter)sensor.Implementation);
            }
        }

        /// <summary>
        /// Updates primary sensors
        /// </summary>
        /// <typeparam name="T">Type of sensor</typeparam>
        /// <param name="sensor">Sensor to consider</param>
        /// <param name="field">Reference to a field, which stores the primary sensor of type <see cref="T"/>.</param>
        /// <param name="sensorImpl">Implementation of sensor.</param>
        private void Update<T>(Sensor sensor, ref T field, T sensorImpl) where T : ISensor {
            if (field == null && sensor.IsEnabled) {
                // no sensor of this type has been set yet
                field = sensorImpl;
            } else {
                if (EqualityComparer<T>.Default.Equals(field, sensorImpl) && sensor.IsEnabled == false) {
                    // search for another sensor for default, which is enabled
                    var sensorDefault = _sensors.FirstOrDefault(s => s.Implementation is T && s.IsEnabled);
                    if (sensorDefault != null)
                        field = (T)Convert.ChangeType(sensorDefault.Implementation, typeof(T));
                    else
                        field = default(T);
                } else if (sensor.IsEnabled) {
                    field = sensorImpl;
                }
            }
        }

        public IGps Gps {
            get;
            private set;
        }

        /// <summary>
        /// Primary heart rate monitor
        /// </summary>
        /// <value>The heart rate monitor.</value>
        public IHeartRateMonitor HeartRateMonitor {
            get;
            private set;
        }

        /// <summary>
        /// Primary speed sensor
        /// </summary>
        /// <value>The speed sensor.</value>
        public ISpeedSensor SpeedSensor {
            get;
            private set;
        }

        /// <summary>
        /// Primary cadence sensor
        /// </summary>
        /// <value>The cadence sensor.</value>
        public ICadenceSensor CadenceSensor {
            get;
            private set;
        }

        private IBikePowerMeter _bikePowerMeter;
        public IBikePowerMeter BikePowerMeter {
            get { return _bikePowerMeter; }
        }

        /// <summary>
        /// System clock
        /// </summary>
        public Clock Clock {
            get;
            private set;
        }

        public IEnumerator<Sensor> GetEnumerator() {
            return _sensors.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
            return _sensors.GetEnumerator();
        }
    }
}
