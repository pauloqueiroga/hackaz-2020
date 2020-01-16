using Microsoft.Extensions.Logging;
using Position4All.DemoPublishingApp.Messages;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Position4All.DemoPublishingApp
{
    internal class ScenarioReader
    {
        private ILogger _logger;
        private string _scenariosPath;
        private string _annotationsPath;
        private Publisher _publisher;
        private AlarmLogger _alarmLogger;
        private IDictionary<double, ExpectedAlarm> _annotations;

        public ScenarioReader(ILogger logger, string scenariosPath, Publisher publisher, AlarmLogger alarmLogger)
        {
            _logger = logger;
            _scenariosPath = scenariosPath;
            _annotationsPath = Path.Combine(_scenariosPath, "annotations/");
            _publisher = publisher;
            _alarmLogger = alarmLogger;
        }

        internal async Task<bool> RunAsync(CancellationToken cancellationToken)
        {
            var completed = await Task.Run(() => Run(cancellationToken)).ConfigureAwait(false);
            return completed;
        }

        internal bool Run(CancellationToken cancellationToken)
        {
            var counter = 0;

            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    _logger.LogInformation($"Using scenarios path: {_scenariosPath}");
                    var currentIndex = 0;

                    while (!cancellationToken.IsCancellationRequested)
                    {
                        counter++;
                        var allFiles = Directory.GetFiles(_scenariosPath);
                        _logger.LogInformation($"Found {allFiles.Length} files");
                        currentIndex %= allFiles.Length;
                        var filePath = allFiles[currentIndex];
                        _logger.LogInformation($"{currentIndex} of {allFiles.Length} Loading all of {filePath}");
                        var scenario = File.ReadAllLines(filePath);

                        var nextAnnotationTimeout = ResetAnnotations(filePath);
                        var state = new State();
                        var readUntil = 100;

                        foreach (var record in scenario)
                        {
                            var (vehicleId, ellapsedMillis, position) = CsvToPosition(record);

                            if (ellapsedMillis > nextAnnotationTimeout)
                            {
                                ExpectedAlarm annotation;
                                (nextAnnotationTimeout, annotation) = PopAnnotation(nextAnnotationTimeout);
                                annotation.CurrentState = state.Clone();
                                _alarmLogger.LogAlarm(annotation);
                            }

                            if (ellapsedMillis > readUntil)
                            {
                                readUntil += 100;
                                _publisher.EnqueueMessage(state.Clone());
                                Task.Delay(100).Wait(cancellationToken);
                            }

                            if (vehicleId == "0")
                            {
                                state.OwnPosition = position;
                                continue;
                            }

                            if (vehicleId == "1")
                            {
                                state.Target1Position = position;
                                continue;
                            }

                            if (vehicleId == "2")
                            {
                                state.Target2Position = position;
                                continue;
                            }

                            if (vehicleId == "3")
                            {
                                state.Target3Position = position;
                                continue;
                            }
                        }

                        currentIndex++;
                    }
                }
                catch (Exception e)
                {
                    _logger.LogError(e.Message);
                    Task.Delay(1000).Wait(cancellationToken);
                }
            }

            _logger.LogInformation($"Went through {counter} files. Cancelled {cancellationToken.IsCancellationRequested}.");
            return !cancellationToken.IsCancellationRequested;
        }

        private (string, double, Position) CsvToPosition(string csv)
        {
            if (string.IsNullOrWhiteSpace(csv))
            {
                _logger.LogError("CSV to parse doesn't contain anything");
                return (null, 0, null);
            }

            var fields = csv.Split(",");

            if (fields.Count() != 7)
            {
                _logger.LogError($"Was expecting 6 fields, found {fields.Count()}");
                return (null, 0, null);
            }

            var id = fields[0];
            var ellapsedMillis = Convert.ToDouble(fields[1]);

            var parsedPosition = new Position
            {
                Latitude = Convert.ToDouble(fields[2]) / 100,
                Longitude = Convert.ToDouble(fields[3]) / 100,
                Altitude = Convert.ToDouble(fields[4]),
                Heading = Convert.ToDouble(fields[5]),
                Velocity = Convert.ToDouble(fields[6]),
            };

            return (id, ellapsedMillis, parsedPosition);
        }

        private double ResetAnnotations(string filePath)
        {
            _annotations = new Dictionary<double, ExpectedAlarm>();
            var fileName = Path.GetFileName(filePath);
            var annotationFilePath = Path.Combine(_annotationsPath, fileName);

            if (!File.Exists(annotationFilePath))
            {
                _logger.LogInformation($"No annotation");
                return double.MaxValue;
            }

            _logger.LogInformation($"Loading annotations from {annotationFilePath}");
            var csv = File.ReadAllLines(annotationFilePath);

            foreach (var record in csv)
            {
                var fields = record.Split(",");

                if (fields == null || fields.Length != 4)
                {
                    continue;
                }

                var alarmId = Guid.NewGuid().ToString("N");
                var raiseAlarm = new ExpectedAlarm
                {
                    AlarmId = alarmId,
                    AlarmRegion = fields[2],
                    AlarmType = fields[3]
                };
                _annotations[Convert.ToDouble(fields[0]) * 1000] = raiseAlarm;
                var clearAlarm = new ExpectedAlarm
                {
                    AlarmId = alarmId,
                    AlarmRegion = fields[2],
                    AlarmType = "clear",
                };
                _annotations[Convert.ToDouble(fields[1]) * 1000] = clearAlarm;
            }

            if (_annotations.Any())
            {
                return _annotations.Keys.First();
            }

            return double.MaxValue;
        }

        private (double next, ExpectedAlarm popped) PopAnnotation(double key)
        {
            var currentAnnotation = _annotations[key];
            _annotations.Remove(key);
            var next = double.MaxValue;

            if (_annotations.Any())
            {
                next = _annotations.Keys.First();
            }

            return (next, popped: currentAnnotation);
        }
    }
}