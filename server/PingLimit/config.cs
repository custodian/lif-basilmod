/*
	BasilMod::PingLimit configuration file
	2015 Basil Semuonov
	
	Adjust message and maximum allowed ping for player connections.

	$BasilMod::pinglimit::maximumPing - sets maximum ping value.
	$BasilMod::pinglimit::message - declares message which will be shown to disconnected player.
	
	You can use Basilmod::pinglimit_stats(); function call to monitor current pings of players on your server.
	
	You can use special macros in message:
		$ping$		will be replaced with player ping value
		$maxping$	will be replaced with maximum allowed ping

	The example message will be shown as follows:
	
	Your ping 620ms is too high!
	Maximum allowed ping on this server is 400ms.
*/

$BasilMod::pinglimit::message = "Your ping $ping$ms is too high!\nMaximum allowed ping on this server is $maxping$ms.\n\nCheck your network settings or increase your bandwidth.";
$BasilMod::pinglimit::maximumPing = 400; //Value in ms.

//Enable ping monitoring while player is connected to server
$BasilMod::pinglimit::monitorEnabled = true;
//Player ping monitoring interval. Value in ms.
$BasilMod::pinglimit::monitorInterval = 1000; 
//Maximum ping value allowed by the server. Value in ms.
$BasilMod::pinglimit::monitorMaxPing = 800;
//If player ping exceeds monitorMaxPing for monitorCount times, player will be disconnected
$BasilMod::pinglimit::monitorCount = 2;

