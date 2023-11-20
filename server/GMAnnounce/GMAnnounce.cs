/*
	BasilMod::GMAnnounce server-side
	2016 Basil Semuonov
*/

package BasilModGMAnnounce {

	function BasilMod::gmannounce_init() {
		if (!isObject($BasilMod::gmannounce::logos)) {
			$BasilMod::gmannounce::logos = new ArrayObject();
		}
		if (!isObject($BasilMod::gmannounce::sounds)) {
			$BasilMod::gmannounce::sounds = new ArrayObject();
		}

		BasilMod::pack_content("basilmod/gmannounce/gmannounce.cs.dso", "basilmod/gmannounce/client/gmannounce.cs.dso");
		BasilMod::pack_content("basilmod/gmannounce/gmannounce.gui.dso", "basilmod/gmannounce/client/gmannounce.gui.dso");
		if (isFile("basilmod/gmannounce/encoding.cs")) {
			BasilMod::pack_content("basilmod/gmannounce/encoding.cs", "basilmod/gmannounce/encoding.cs");
		}

		BasilMod.gmannounce_loadFiles($BasilMod::gmannounce::sounds, "sound/*.ogg");
		BasilMod.gmannounce_loadFiles($BasilMod::gmannounce::logos, "logo/*.png");

		//Enable autoload
		BasilMod.pack_autoloadClientMod("gmannounce");
	}

	function BasilMod::gmannounce_loadFiles(%this, %array, %pattern) {
		%array.empty();
		%basePath = filePath(%pattern) @ "/";
		%pattern = "basilmod/gmannounce/client/" @ %pattern;

		%path = filePath(%pattern) @ "/";
		for (%file = findFirstFileMultiExpr(%pattern); %file !$= ""; %file = findNextFileMultiExpr(%pattern)) {
			//remove sourcedir pattern from filename for taget name
			%filename = strreplace(%file, %path, "");
			%targetFile = "basilmod/gmannounce/" @ %basePath @ %filename;
			%baseName = fileBase(%filename);
			hack("Found", %baseName, %targetFile, %file);

			BasilMod::pack_content(%targetFile, %file);
			%array.add(%baseName, %targetFile);
		}
	}

	function BasilMod::netCmd_GMAnnounceArtData(%this, %client) {
		%logos = "";
		%sounds = "";

		%count = $BasilMod::gmannounce::logos.count();
		for(%i=0;%i<%count;%i++) {
			%logoName = $BasilMod::gmannounce::logos.getKey(%i);
			%logoPath = $BasilMod::gmannounce::logos.getValue(%i);

			if (%i) {
				%logos = %logos NL %logoName NL %logoPath;
			} else {
				%logos = %logoName NL %logoPath;
			}
		}

		%count = $BasilMod::gmannounce::sounds.count();
		for(%i=0;%i<%count;%i++) {
			%soundName = $BasilMod::gmannounce::sounds.getKey(%i);
			%soundPath = $BasilMod::gmannounce::sounds.getValue(%i);

			if (%i) {
				%sounds = %sounds NL %soundName NL %soundPath;
			} else {
				%sounds = %soundName NL %soundPath;
			}
		}
		BasilMod.sendCommand(%client, "GMAnnounceArtData", %logos, %sounds);
	}

	function BasilMod::gmannounce_makeAnnounce(%this, %config, %message) {
		%count = ClientGroup.getCount();
		for (%i = 0; %i < %count; %i++)
		{
			%cl = ClientGroup.getObject(%i);
			BasilMod.sendCommand(%cl, "GMAnnounceShowAnnounce", %config, %message);
		}
	}

	function BasilMod::netCmd_GMAnnounceMakeAnnounce(%this, %client, %config, %message) {
		if (!%client.isGM()) {
			BasilMod.sendCommand(%client, "GMAnnounceFailed", "Only GM can make server announcements");
			return;
		}
		BasilMod.gmannounce_makeAnnounce(%config, %message);
		BasilMod.sendCommand(%client, "GMAnnounceSuccess");
	}
};

if (!isObject(BasilMod) || $BasilMod::pack::version < 4) {
	error("BasilMod::GMAnnounce requires BasilMod::Pack v4+ installed and running!");
} else {
	activatePackage("BasilModGMAnnounce");
	BasilMod::gmannounce_init();
	echo("BasilMod::GMAnnounce v5 loaded.");
}