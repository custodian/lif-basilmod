/*
	BasilMod::PingLimit server side.
	2017 Basil Semuonov
*/

if (!isObject(BasilMod)) {
	new ScriptObject(BasilMod);
}

package BasilModPingLimit
{
	function BasilMod::pinglimit_init()
	{
		$BasilMod::pinglimit::debug = false;
		$BasilMod::pinglimit::version = 4;

		//Enable ping monitoring while player is connected to server
		$BasilMod::pinglimit::monitorEnabled = true;
		//Initial delay before ping check starts
		$BasilMod::pinglimit::monitorDelay = 10000; //Value in ms
		//Player ping monitoring interval. Value in ms.
		$BasilMod::pinglimit::monitorInterval = 1000;
		//Maximum ping value allowed by the server. Value in ms.
		$BasilMod::pinglimit::monitorMaxPing = 800;
		//If player ping exceeds monitorMaxPing for monitorCount times, player will be disconnected
		$BasilMod::pinglimit::monitorCount = 2;

		if (isFile("basilmod/pinglimit/config.cs")) {
			exec("basilmod/pinglimit/config.cs");
		}
	}

	function GameConnection::onConnect(%client, %name)
	{
		BasilMod.schedule($BasilMod::pinglimit::monitorDelay, "pinglimit_connectionCheck", %client);
		%client.bmPingLimitCount = 0;
		SendServerUUIDEvent(%client);
		hack(%client, %name);
	}

	function BasilMod::pinglimit_connectionCheck(%this, %client)
	{
		if (!isObject(%client)) {
			return;
		}
		%ping = %client.getPing();
		if ($BasilMod::pinglimit::debug) {
			hack(%client, "PING", %ping);
		}
		if (%ping > $BasilMod::pinglimit::maximumPing) {
			%message = strreplace($BasilMod::pinglimit::message, "$ping$", %ping);
			%message = strreplace(%message, "$maxping$", $BasilMod::pinglimit::maximumPing);
			%client.delete("\n\n" @ %message);
			return;
		}

		if ($BasilMod::pinglimit::monitorEnabled) {
			if (%ping >= $BasilMod::pinglimit::monitorMaxPing) {
				%client.bmPingLimitCount++;
			} else {
				%client.bmPingLimitCount--;
			}
			if (%client.bmPingLimitCount < 0 ) {
				%client.bmPingLimitCount = 0;
			}

			if (%client.bmPingLimitCount > $BasilMod::pinglimit::monitorCount) {
				%message = strreplace($BasilMod::pinglimit::message, "$ping$", %ping);
				%message = strreplace(%message, "$maxping$", $BasilMod::pinglimit::maximumPing);
				%client.delete("\n\n" @ %message);
				return;
			}

			BasilMod.schedule($BasilMod::pinglimit::monitorInterval, "pinglimit_connectionCheck", %client);
		}
	}

	function BasilMod::pinglimit_stats(%this)
	{
		%count = ClientGroup.getCount();
		for (%i = 0; %i < %count; %i++)
		{
			%cl = ClientGroup.getObject(%i);
			%accountId = %cl.accountId;
			%charId = %cl.charId;
			if (!%charId) {
				%charId = "NotSelected";
			}
			warn("AccountId:" SPC %accountId SPC "Player charId:" SPC %charId SPC "current ping is", %cl.getPing());
			if (%this.isMethod("motd_sendSystemMessage")) {
				if (%cl.isGM()) {
					%this.motd_sendSystemMessage(%cl, %message);
				}
			}
		}
	}
};

activatePackage(BasilModPingLimit);
BasilMod::pinglimit_init();
echo("BasilMod::PingLimit: Maximum allowed ping is" SPC $BasilMod::pinglimit::maximumPing SPC "ms.");
echo("BasilMod::PingLimit v" @ $BasilMod::pinglimit::version @ " loaded.");