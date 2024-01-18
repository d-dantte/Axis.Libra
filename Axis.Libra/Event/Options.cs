using Axis.Luna.Common;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Axis.Libra.Event
{
    /// <summary>
    /// 
    /// </summary>
    public class OptionsBuilder
    {
        private TaskScheduler? _taskScheduler;
        private CancellationTokenSource? _cancellationTokenSource;
        private TaskCreationOptions _taskCreationOption;

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static OptionsBuilder NewBuilder() => new();

        private OptionsBuilder()
        {
            _taskScheduler = null;
            _cancellationTokenSource = null;
            _taskCreationOption = default;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="taskScheduler"></param>
        /// <returns></returns>
        public OptionsBuilder WithScheduler(TaskScheduler taskScheduler)
        {
            ArgumentNullException.ThrowIfNull(taskScheduler);
            _taskScheduler = taskScheduler;
            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tokenSource"></param>
        /// <returns></returns>
        public OptionsBuilder WithCancellationTokenSource(CancellationTokenSource tokenSource)
        {
            ArgumentNullException.ThrowIfNull(tokenSource);
            _cancellationTokenSource = tokenSource;
            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="optoins"></param>
        /// <returns></returns>
        public OptionsBuilder WithTaskCreationOptions(TaskCreationOptions optoins)
        {
            ArgumentNullException.ThrowIfNull(optoins);
            _taskCreationOption = optoins;
            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Options Build() => new(
            _cancellationTokenSource!,
            _taskScheduler!,
            _taskCreationOption);
    }

    /// <summary>
    /// 
    /// </summary>
    public readonly struct Options:
        IDefaultValueProvider<Options>
    {
        /// <summary>
        /// 
        /// </summary>
        public TaskScheduler TaskScheduler { get; }

        /// <summary>
        /// 
        /// </summary>
        public CancellationTokenSource CancellationTokenSource { get; }

        /// <summary>
        /// 
        /// </summary>
        public TaskCreationOptions TaskCreationOptions { get; }

        /// <summary>
        /// 
        /// </summary>
        public bool IsDefault
            => TaskScheduler is null
            && CancellationTokenSource is null
            && TaskCreationOptions.Equals(default(TaskCreationOptions));

        /// <summary>
        /// 
        /// </summary>
        public static Options Default => default;

        public Options(
            CancellationTokenSource tokenSource,
            TaskScheduler scheduler,
            TaskCreationOptions taskCreationOptions)
        {
            ArgumentNullException.ThrowIfNull(tokenSource);
            ArgumentNullException.ThrowIfNull(scheduler);

            TaskScheduler = scheduler;
            TaskCreationOptions = taskCreationOptions;
            CancellationTokenSource = tokenSource;
        }
    }
}
