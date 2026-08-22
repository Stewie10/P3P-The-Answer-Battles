using Microsoft.Extensions.Configuration;
using P3PPC.Expansion.TheAnswer;
using P3PPC.Expansion.TheAnswer.Components;
using P3PPC.Expansion.TheAnswer.Configuration;
using P3PPC.Expansion.TheAnswer.FunctionDelegates;
using P3PPC.Expansion.TheAnswer.Template;
using Reloaded.Hooks.Definitions;
using Reloaded.Hooks.Definitions.Enums;
using Reloaded.Hooks.Definitions.X64;
using Reloaded.Hooks.ReloadedII.Interfaces;
using Reloaded.Memory;
using Reloaded.Memory.SigScan.ReloadedII.Interfaces;
using Reloaded.Mod.Interfaces;
using Reloaded.Universal.Localisation.Framework.Interfaces;
using RyoTune.Reloaded.Inis;
using RyoTune.Reloaded.Scans;
using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel.Design;
using System.Drawing;
using System.Security.Cryptography.X509Certificates;
using P3PPC.Expansion.TheAnswer.BossBtlData;
using IReloadedHooks = Reloaded.Hooks.ReloadedII.Interfaces.IReloadedHooks;

namespace P3PPC.Expansion.TheAnswer
{
    /// <summary>
    /// Your mod logic goes here.
    /// </summary>
    public unsafe class Mod : ModBase // <= Do not Remove.
    {
        /// <summary>
        /// Provides access to the mod loader API.
        /// </summary>
        private readonly IModLoader _modLoader;

        /// <summary>
        /// Provides access to the Reloaded.Hooks API.
        /// </summary>
        /// <remarks>This is null if you remove dependency on Reloaded.SharedLib.Hooks in your mod.</remarks>
        private readonly IReloadedHooks? _hooks;

        /// <summary>
        /// Provides access to the Reloaded logger.
        /// </summary>
        private readonly ILogger _logger;

        /// <summary>
        /// Entry point into the mod, instance that created this class.
        /// </summary>
        private readonly IMod _owner;

        /// <summary>
        /// Provides access to this mod's configuration.
        /// </summary>
        private Config _configuration;

        /// <summary>
        /// The configuration of the currently executing mod.
        /// </summary>
        private readonly IModConfig _modConfig;

        private readonly P3PInfo? _p3pInfo;

        private readonly Memory _memory;

        public Mod(ModContext context)
        {
            _modLoader = context.ModLoader;
            _hooks = context.Hooks;
            _logger = context.Logger;
            _owner = context.Owner;
            _configuration = context.Configuration;
            _modConfig = context.ModConfig;
            _memory = Memory.Instance;

            Project.Initialize(_modConfig, _modLoader, _logger);

            if (!_modLoader.GetController<IStartupScanner>().TryGetTarget(out var startupScanner))
            {
                Log.Error($"Unable to get controller for Reloaded SigScan Library");
            }
            else
            {
                _p3pInfo = new P3PInfo(startupScanner, _hooks!, _memory);
            }

#if DEBUG
            // Attaches debugger in debug mode; ignored in release.
            Debugger.Launch();
#endif

            if (_p3pInfo != null)
            {
                BossBtlUnitHook.HookInfoProvider(_p3pInfo);
                BossBtlUnitAnimHook.HookInfoProvider(_p3pInfo);
                BossBtlBEDFilesHook.HookInfoProvider(_p3pInfo);
                BossBtlCombatInfoHook.HookInfoProvider(_p3pInfo);
            }
        }
        #region Private Helper Methods

        #endregion

        #region Standard Overrides
        public override void ConfigurationUpdated(Config configuration)
        {
            // Apply settings from configuration.
            // ... your code here.
            _configuration = configuration;
            _logger.WriteLine($"[{_modConfig.ModId}] Config Updated: Applying");
        }
        #endregion

        #region For Exports, Serialization etc.
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public Mod() { }
#pragma warning restore CS8618
        #endregion
    }
}