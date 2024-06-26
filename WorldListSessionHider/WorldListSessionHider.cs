using HarmonyLib;
using ResoniteModLoader;
using FrooxEngine;
using System.Reflection;
using System;
using System.Collections.Generic;
using System.Linq;
using SkyFrost.Base;
using Elements.Core;
using FrooxEngine.UIX;

namespace WorldListSessionHider
{
	public class WorldListSessionHider : ResoniteMod
	{
		public override string Name => "WorldListSessionHider";
		public override string Author => "Nytra";
		public override string Version => "1.2.0";
		public override string Link => "https://github.com/Nytra/ResoniteWorldListSessionHider";

		public static ModConfiguration Config;

		[AutoRegisterConfigKey]
		private static ModConfigurationKey<bool> MOD_ENABLED = new ModConfigurationKey<bool>("MOD_ENABLED", "Mod activated:", () => true);
		[AutoRegisterConfigKey]
		private static ModConfigurationKey<dummy> DUMMY_0 = new ModConfigurationKey<dummy>("DUMMY_0", "<size=0></size>", () => new dummy());
		[AutoRegisterConfigKey]
		private static ModConfigurationKey<dummy> DUMMY_0_1 = new ModConfigurationKey<dummy>("DUMMY_0_1", "<color=green>[FILTERS]</color>", () => new dummy());
		[AutoRegisterConfigKey]
		private static ModConfigurationKey<bool> USE_FILTERS = new ModConfigurationKey<bool>("USE_FILTERS", "Use filters:", () => true);
		[AutoRegisterConfigKey]
		private static ModConfigurationKey<dummy> DUMMY_0_2 = new ModConfigurationKey<dummy>("DUMMY_0_2", "<size=0></size>", () => new dummy());
		[AutoRegisterConfigKey]
		private static ModConfigurationKey<string> HOST_USERIDS = new ModConfigurationKey<string>("HOST_USERIDS", "Host User IDs to filter:", () => "");
		[AutoRegisterConfigKey]
		private static ModConfigurationKey<string> HOST_USERNAMES = new ModConfigurationKey<string>("HOST_USERNAMES", "Host Usernames to filter:", () => "");
		[AutoRegisterConfigKey]
		private static ModConfigurationKey<string> SESSION_IDS = new ModConfigurationKey<string>("SESSION_IDS", "Session IDs to filter:", () => "");
		[AutoRegisterConfigKey]
		private static ModConfigurationKey<string> WORLD_IDS = new ModConfigurationKey<string>("WORLD_IDS", "Composite World Record IDs / World Record URLs to filter:", () => "");
		[AutoRegisterConfigKey]
		private static ModConfigurationKey<dummy> DUMMY_1 = new ModConfigurationKey<dummy>("DUMMY_1", "<i><color=gray>All of these can be comma-separated to store multiple values.</color></i>", () => new dummy());
		[AutoRegisterConfigKey]
		private static ModConfigurationKey<dummy> DUMMY_1_1 = new ModConfigurationKey<dummy>("DUMMY_1_1", "<i><color=gray>e.g: U-Cheese, U-spaghet, U-OwO</color></i>", () => new dummy());

		// session ID examples

		[AutoRegisterConfigKey]
		private static ModConfigurationKey<dummy> DUMMY_1_1_1b = new ModConfigurationKey<dummy>("DUMMY_1_1_1b", "<size=0></size>", () => new dummy());
		[AutoRegisterConfigKey]
		private static ModConfigurationKey<dummy> DUMMY_1_2b = new ModConfigurationKey<dummy>("DUMMY_1_2b", "<i><color=gray>Acceptable Session ID Examples:</color></i>", () => new dummy());
		[AutoRegisterConfigKey]
		private static ModConfigurationKey<dummy> DUMMY_1_2_1b = new ModConfigurationKey<dummy>("DUMMY_1_2_1b", "<i><color=gray>S-U-Nytra:AmazingSession</color></i>", () => new dummy());
		[AutoRegisterConfigKey]
		private static ModConfigurationKey<dummy> DUMMY_1_2_2b = new ModConfigurationKey<dummy>("DUMMY_1_2_2b", "<i><color=gray>S-blah-blah</color></i>", () => new dummy());

		// world ID examples

		[AutoRegisterConfigKey]
		private static ModConfigurationKey<dummy> DUMMY_1_1_1 = new ModConfigurationKey<dummy>("DUMMY_1_1_1", "<size=0></size>", () => new dummy());
		[AutoRegisterConfigKey]
		private static ModConfigurationKey<dummy> DUMMY_1_2 = new ModConfigurationKey<dummy>("DUMMY_1_2", "<i><color=gray>Acceptable World ID Examples:</color></i>", () => new dummy());
		[AutoRegisterConfigKey]
		private static ModConfigurationKey<dummy> DUMMY_1_2_1 = new ModConfigurationKey<dummy>("DUMMY_1_2_1", "<i><color=gray>resrec:///U-Nytra/R-blah-blah</color></i>", () => new dummy());
		[AutoRegisterConfigKey]
		private static ModConfigurationKey<dummy> DUMMY_1_2_2 = new ModConfigurationKey<dummy>("DUMMY_1_2_2", "<i><color=gray>U-Nytra/R-blah-blah</color></i>", () => new dummy());
		[AutoRegisterConfigKey]
		private static ModConfigurationKey<dummy> DUMMY_1_2_3 = new ModConfigurationKey<dummy>("DUMMY_1_2_3", "<i><color=gray>U-Nytra:R-blah-blah</color></i>", () => new dummy());


		[AutoRegisterConfigKey]
		private static ModConfigurationKey<dummy> DUMMY_1_3 = new ModConfigurationKey<dummy>("DUMMY_1_3", "<size=0></size>", () => new dummy());
		[AutoRegisterConfigKey]
		private static ModConfigurationKey<bool> HIDE_FILTERED_SESSIONS_COMPLETELY = new ModConfigurationKey<bool>("HIDE_FILTERED_SESSIONS_COMPLETELY", "Hide filtered sessions completely:", () => false);
		[AutoRegisterConfigKey]
		private static ModConfigurationKey<dummy> DUMMY_2 = new ModConfigurationKey<dummy>("DUMMY_2", "<size=0></size>", () => new dummy());
		[AutoRegisterConfigKey]
		private static ModConfigurationKey<dummy> DUMMY_2_1 = new ModConfigurationKey<dummy>("DUMMY_2_1", "<color=green>[STUCK SESSIONS]</color>", () => new dummy());
		[AutoRegisterConfigKey]
		private static ModConfigurationKey<bool> HIDE_DEAD_SESSIONS = new ModConfigurationKey<bool>("HIDE_DEAD_SESSIONS", "Hide stuck sessions:", () => false);
		//[AutoRegisterConfigKey]
		//private static ModConfigurationKey<int> DEAD_SESSION_LAST_UPDATE_HOURS = new ModConfigurationKey<int>("DEAD_SESSION_LAST_UPDATE_HOURS", "If session has not updated for this number of hours, consider it possibly stuck:", () => 24, valueValidator: v => v > 0);
		[AutoRegisterConfigKey]
		private static ModConfigurationKey<bool> HIDE_DEAD_SESSIONS_COMPLETELY = new ModConfigurationKey<bool>("HIDE_DEAD_SESSIONS_COMPLETELY", "Hide stuck sessions completely:", () => false);
		[AutoRegisterConfigKey]
		private static ModConfigurationKey<dummy> DUMMY_2_2 = new ModConfigurationKey<dummy>("DUMMY_2_2", "<i><color=gray>A stuck session is one that shows up in the browser but doesn't actually exist and cannot be joined.</color></i>", () => new dummy());
		[AutoRegisterConfigKey]
		private static ModConfigurationKey<dummy> DUMMY_2_3 = new ModConfigurationKey<dummy>("DUMMY_2_3", "<i><color=orange>It is unknown if this feature is actually useful in Resonite, so it is turned off by default.</color></i>", () => new dummy());
		//[AutoRegisterConfigKey]
		//private static ModConfigurationKey<dummy> DUMMY_6 = new ModConfigurationKey<dummy>("DUMMY_6", "<size=0></size>", () => new dummy());
		//[AutoRegisterConfigKey]
		//private static ModConfigurationKey<bool> HIDE_EXPIRED_SESSIONS = new ModConfigurationKey<bool>("HIDE_EXPIRED_SESSIONS", "Hide expired sessions:", () => true);
		//[AutoRegisterConfigKey]
		//private static ModConfigurationKey<int> LAST_UPDATE_MAX_DAYS = new ModConfigurationKey<int>("LAST_UPDATE_MAX_DAYS", "If session has not updated for this number of days, consider it expired:", () => 90, valueValidator: v => v > 0);
		//[AutoRegisterConfigKey]
		//private static ModConfigurationKey<bool> HIDE_EXPIRED_SESSIONS_COMPLETELY = new ModConfigurationKey<bool>("HIDE_EXPIRED_SESSIONS_COMPLETELY", "Hide expired sessions completely:", () => false);
		//[AutoRegisterConfigKey]
		//private static ModConfigurationKey<dummy> DUMMY_7 = new ModConfigurationKey<dummy>("DUMMY_7", "<i><color=gray>Meant to hide sessions that have not updated in a very long time.</color></i>", () => new dummy());
		[AutoRegisterConfigKey]
		private static ModConfigurationKey<bool> HIDE_ENDED_SESSIONS = new ModConfigurationKey<bool>("HIDE_ENDED_SESSIONS", "Hide ended sessions (Sessions without URLs):", () => false, internalAccessOnly: true);
		[AutoRegisterConfigKey]
		private static ModConfigurationKey<bool> HIDE_ENDED_SESSIONS_COMPLETELY = new ModConfigurationKey<bool>("HIDE_ENDED_SESSIONS_COMPLETELY", "Hide ended sessions completely:", () => false, internalAccessOnly: true);
		[AutoRegisterConfigKey]
		private static ModConfigurationKey<bool> EXTRA_LOGGING = new ModConfigurationKey<bool>("EXTRA_LOGGING", "Enable extra debug logging:", () => false, internalAccessOnly: true);

		public override void OnEngineInit()
		{
			Harmony harmony = new Harmony("owo.Nytra.WorldListSessionHider");
			Config = GetConfiguration();
			Config.OnThisConfigurationChanged += (configChangedEvent) =>
			{
				if (configChangedEvent.Key == EXTRA_LOGGING && Config.GetValue(EXTRA_LOGGING))
				{
					Debug("Logging configured strings...");
					foreach (string s in Config.GetValue(HOST_USERIDS)?.Split(','))
					{
						Debug($"UserID: \"{s}\"");
					}
					foreach (string s in Config.GetValue(HOST_USERNAMES)?.Split(','))
					{
						Debug($"Username: \"{s}\"");
					}
					foreach (string s in Config.GetValue(SESSION_IDS)?.Split(','))
					{
						Debug($"SessionID: \"{s}\"");
					}
					foreach (string s in Config.GetValue(WORLD_IDS)?.Split(','))
					{
						Debug($"WorldID: \"{s}\"");
					}
					Debug("Done.");
				}
			};
			harmony.PatchAll();
		}

		private static Type worldThumbnailItemType = typeof(LegacyWorldThumbnailItem);

		private static MethodInfo getBestSessionMethod = AccessTools.Method(typeof(LegacyWorldItem), "GetBestSession");

		private static MethodInfo updateThumbnailMethod = AccessTools.Method(worldThumbnailItemType, "UpdateThumbnailURL");
		private static FieldInfo nameTextField = AccessTools.Field(worldThumbnailItemType, "_nameText");
		private static FieldInfo detailTextField = AccessTools.Field(worldThumbnailItemType, "_detailText");
		private static FieldInfo counterTextField = AccessTools.Field(worldThumbnailItemType, "_counterText");
		private static FieldInfo deferredThumbnailField = AccessTools.Field(worldThumbnailItemType, "_deferredThumbnailUrl");
		//private static FieldInfo thumbnailTextureField = AccessTools.Field(worldThumbnailItemType, "_thumbnailTexture");

		private static FrooxEngine.User LocalUser => Engine.Current.WorldManager.FocusedWorld.LocalUser ?? Userspace.UserspaceWorld.LocalUser;

		private static List<SessionInfo> sessionsForWorldId = new List<SessionInfo>();

		enum HideReason
		{
			Filtered,
			Stuck,
			Ended
		}

		//private static bool ReceivedContactRequestInSession(SessionInfo sessionInfo)
		//{
		//	foreach (SessionUser user in sessionInfo.SessionUsers)
		//	{
		//		Friend friend = Engine.Current.Cloud.Friends.FindFriend((Friend f) => f.FriendUserId == user.UserID && f.FriendStatus == FriendStatus.Requested);
		//		if (friend != null) return true;
		//	}
		//	return false;
		//}

		//private static void PrintSessionUsers(SessionInfo sessionInfo)
		//{
		//	foreach(SessionUser user in sessionInfo.SessionUsers)
		//	{
		//		Debug($"UserID: {user.UserID} Username: {user.Username}");
		//	}
		//}

		private static bool WorldIdFilter(SessionInfo sessionInfo)
		{
			if (string.IsNullOrWhiteSpace(sessionInfo.CorrespondingWorldId?.ToString())) return false;

			string worldIds = Config.GetValue(WORLD_IDS);
			if (string.IsNullOrWhiteSpace(worldIds)) return false;

			foreach (var str in worldIds.Split(','))
			{
				string trimmed = str.Trim();
				if (string.IsNullOrWhiteSpace(trimmed)) continue;

				if (trimmed == sessionInfo.CorrespondingWorldId.ToString()) return true;

				// try to turn it into a valid composite record id
				string newstr = trimmed;
				string recordSchemeStr = PlatformProfile.RESONITE.RecordScheme + ":///";
				if (newstr.StartsWith(recordSchemeStr))
				{
					newstr = newstr.Remove(0, recordSchemeStr.Length);
				}
				newstr = newstr.Replace('/', ':');
				if (newstr == sessionInfo.CorrespondingWorldId.ToString()) return true;
			}

			return false;
		}

		private static bool ShouldHideSession(SessionInfo sessionInfo, out HideReason? reason)
		{
			string sessionIds = Config.GetValue(SESSION_IDS);
			string hostUserIds = Config.GetValue(HOST_USERIDS);
			string hostUsernames = Config.GetValue(HOST_USERNAMES);

			reason = null;

			// Don't hide sessions that are currently opened worlds
			if (Engine.Current.WorldManager.Worlds.Any((World w) => w.SessionId == sessionInfo.SessionId)) return false;

			// If the session is not an opened world, but the local user is supposedly in the session then it's a stuck session
			// or if the session has present users but hasn't updated for 30 minutes or more
			if (Config.GetValue(HIDE_DEAD_SESSIONS) &&
				(sessionInfo.SessionUsers.Any((SessionUser user) => (user.UserID ?? user.Username) == (LocalUser.UserID ?? LocalUser.UserName)) ||
				(sessionInfo.SessionUsers.Any((SessionUser user) => user.IsPresent) && DateTime.UtcNow.Subtract(sessionInfo.LastUpdate).TotalMinutes >= 30)))
			{
				reason = HideReason.Stuck;
				return true;
			}
			// Check the config values to see if this session should be filtered
			else if (Config.GetValue(USE_FILTERS) &&
				((!string.IsNullOrWhiteSpace(sessionIds) && sessionIds.Split(',').Select(str => str.Trim()).Contains(sessionInfo.SessionId)) ||
				(!string.IsNullOrWhiteSpace(hostUserIds) && !string.IsNullOrWhiteSpace(sessionInfo.HostUserId) && hostUserIds.Split(',').Select(str => str.Trim()).Contains(sessionInfo.HostUserId)) ||
				(!string.IsNullOrWhiteSpace(hostUsernames) && hostUsernames.Split(',').Select(str => str.Trim()).Contains(sessionInfo.HostUsername)) ||
				WorldIdFilter(sessionInfo))
				)
			{
				reason = HideReason.Filtered;
				return true;
			}
			// If the session has ended (Has no URLs)
			else if (Config.GetValue(HIDE_ENDED_SESSIONS) && sessionInfo.HasEnded)
			{
				reason = HideReason.Ended;
				return true;
			}

			return false;
		}

		private static void ProcessWorldThumbnailItem(LegacyWorldThumbnailItem worldThumbnailItem, string debugString = "")
		{
			SessionInfo sessionInfo = Engine.Current.Cloud.Sessions.TryGetInfo(worldThumbnailItem.WorldOrSessionId);

			if (sessionInfo == null)
			{
				sessionsForWorldId.Clear();
				sessionsForWorldId.TrimExcess();
				Engine.Current.Cloud.Sessions.GetSessionsForWorldId(RecordId.TryParse(worldThumbnailItem.WorldOrSessionId), sessionsForWorldId);
				sessionInfo = (SessionInfo)getBestSessionMethod.Invoke(worldThumbnailItem, new object[] { sessionsForWorldId });
				if (sessionInfo == null) return;
			}

			if (Config.GetValue(EXTRA_LOGGING))
			{
				Debug(new string('=', 30));
				Debug("debugString: " + debugString);
				Debug($"Host UserID: \"{sessionInfo.HostUserId}\" Host Username: \"{sessionInfo.HostUsername}\" SessionID: \"{sessionInfo.SessionId}\" WorldID: \"{sessionInfo.CorrespondingWorldId?.ToString() ?? "NULL"}\"");
				foreach (string url in sessionInfo.SessionURLs)
				{
					Debug($"URL: {url}");
				}
				Debug($"LastUpdate (UTC): {sessionInfo.LastUpdate}");
			}

			HideReason? reason;
			if (ShouldHideSession(sessionInfo, out reason))
			{
				string nameText;
				bool configValue;
				bool flag = true;
				string logString = "Hiding session: \"" + sessionInfo.Name + "\" Reason: ";
				switch (reason)
				{
					case HideReason.Stuck:
						nameText = "<i>[STUCK]</i>";
						configValue = Config.GetValue(HIDE_DEAD_SESSIONS_COMPLETELY);
						logString += "Stuck.";
						break;
					case HideReason.Filtered:
						nameText = "<i>[Filtered]</i>";
						configValue = Config.GetValue(HIDE_FILTERED_SESSIONS_COMPLETELY);
						logString += "Filtered.";
						break;
					case HideReason.Ended:
						nameText = "<i>[ENDED]</i>";
						configValue = Config.GetValue(HIDE_ENDED_SESSIONS_COMPLETELY);
						logString += "Ended.";
						break;
					default:
						nameText = "<i>[UNKNOWN]</i>";
						configValue = false;
						logString += "Unknown.";
						break;
				}
				Debug(logString);
				Hide(worldThumbnailItem, nameText, configValue && flag);
			}
		}

		private static void Hide(LegacyWorldThumbnailItem worldThumbnailItem, string nameTextValue = "<i>[HIDDEN]</i>", bool hideCompletely = false)
		{
			updateThumbnailMethod.Invoke(worldThumbnailItem, new object[] { OfficialAssets.Skyboxes.Thumbnails.NoThumbnail });
			var nameText = (SyncRef<Text>)nameTextField.GetValue(worldThumbnailItem);
			nameText.Target.Content.Value = nameTextValue;
			var detailText = (SyncRef<Text>)detailTextField.GetValue(worldThumbnailItem);
			detailText.Target.Content.Value = "<i>...</i>";
			var counterText = (SyncRef<Text>)counterTextField.GetValue(worldThumbnailItem);
			counterText.Target.Content.Value = "<i>...</i>";
			deferredThumbnailField.SetValue(worldThumbnailItem, OfficialAssets.Skyboxes.Thumbnails.NoThumbnail);

			if (hideCompletely)
			{
				worldThumbnailItem.Slot.ActiveSelf = false;
			}
		}

		[HarmonyPatch(typeof(LegacyWorldThumbnailItem), "UpdateInfo")]
		class WorldListSessionHiderPatch
		{
			public static void Postfix(LegacyWorldThumbnailItem __instance, FrooxEngine.Store.Record record, IReadOnlyList<SessionInfo> sessions, IReadOnlyList<World> openedWorlds)
			{
				if (!Config.GetValue(MOD_ENABLED)) return;

				ProcessWorldThumbnailItem(__instance, "Called from UpdateInfo");
			}
		}

		[HarmonyPatch(typeof(LegacyWorldThumbnailItem), "OnActivated")]
		class WorldListSessionHiderPatch2
		{
			public static void Postfix(LegacyWorldThumbnailItem __instance)
			{
				if (!Config.GetValue(MOD_ENABLED)) return;

				__instance.RunSynchronously(delegate { ProcessWorldThumbnailItem(__instance, "Called from OnActivated"); });
			}
		}
	}
}