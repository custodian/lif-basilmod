/*
	BasilMod::GMAnnounce client-side
	2016 Basil Semuonov
*/

package BasilModGMAnnounce {
	function BasilMod::gmannounce_init() {
		//Reload GUI
		exec("basilmod/gmannounce/gmannounce.gui");
		//load utf32 encoding config
		if (isFile("basilmod/gmannounce/encoding.cs")) {
			exec("basilmod/gmannounce/encoding.cs");
		}
		
		BasilMod.pack_registerActionBind("GMAnnounce", "MakeAnnouncement", "Make new announcement (GM only)", keyboard, "F5", toggleBasilModGMAnnounceWindow);
	}
	
	function BasilMod::gmannounce_utf32_expand(%this, %source)
	{
		%result = %source;
		%original = $BasilMod::GMAnnounce::utf8chars;
		for (%i = 0; %i < strlen(%original); %i++) {
			%char = getSubStr(%original, %i, 1);
			%result = strreplace(%result, %char, "\\u" @ strasc(%char));
		}
		return %result;
	}
	
	function BasilMod::gmannounce_utf32_collapse(%this, %source)
	{
		%result = %source;
		%original = $BasilMod::GMAnnounce::utf8chars;
		for (%i = 0; %i < strlen(%original); %i++) {
			%char = getSubStr(%original, %i, 1);
			%result = strreplace(%result, "\\u" @ strasc(%char), %char);
		}
		return %result;
	}
	
	function toggleBasilModGMAnnounceWindow(%val) {
		if(!%val) {
			return;
		}
		if (isObject(BMGMAnnounceGuiWindow)) {
			BMGMAnnounceGuiWindow.delete();
		} else {
			%window = BasilMod::gmannounce_guiCreateAnnounceWindow();
			BasilMod.sendCommand("GMAnnounceArtData");
			%window.open();
		}
	}
	
	function BasilMod::gmannounce_makeAnnounceConfig(%this) {
		%logo = "";
		%sound = "";
		%soundId = BMGMAnnounceGuiWindow-->soundsList.getSelected();
		if (%soundId > -1 && %soundId < $BasilMod::gmannounce::sounds.count()){
			%sound = $BasilMod::gmannounce::sounds.getValue(%soundId);
		}
		%logoId = BMGMAnnounceGuiWindow-->logosList.getSelected();
		if (%logoId > -1 && %logoId < $BasilMod::gmannounce::logos.count()){
			%logo = $BasilMod::gmannounce::logos.getValue(%logoId);
		}
		%config = %logo NL %sound;
		return %config;
	}
	
	function BasilMod::gmannounce_testAnnounce(%this) {
		%message = BasilMod.gmannounce_utf32_expand(BMGMAnnounceGuiWindow-->messageText.getText());
		%config = BasilMod.gmannounce_makeAnnounceConfig();
		BasilMod.netCmd_GMAnnounceShowAnnounce(%config, %message);
	}
	
	function BasilMod::gmannounce_makeAnnounce(%this) {
		%message = BasilMod.gmannounce_utf32_expand(BMGMAnnounceGuiWindow-->messageText.getText());
		%config = BasilMod.gmannounce_makeAnnounceConfig();
		BasilMod.sendCommand("GMAnnounceMakeAnnounce", %config, %message);
	}
	
	function BasilMod::netCmd_GMAnnounceFailed(%this, %message) {
		MessageBoxOK("Announcement failed", %message, "");
	}
	
	function BasilMod::netCmd_GMAnnounceSuccess(%this) {
		if (isObject(BMGMAnnounceGuiWindow)) {
			MessageBoxYesNo("Announcement", "Close GMAnnouncement?", "BMGMAnnounceGuiWindow.close();", "");
		}
	}

	function BasilMod::netCmd_GMAnnounceArtData(%this, %logos, %sounds) {
		if (!isObject(BMGMAnnounceGuiWindow)) return;
		
		if (!isObject($BasilMod::gmannounce::logos)) {
			$BasilMod::gmannounce::logos = new ArrayObject();
		}
		$BasilMod::gmannounce::logos.empty();
		BMGMAnnounceGuiWindow-->logosList.add("default", $BasilMod::gmannounce::logos.count());
		$BasilMod::gmannounce::logos.add("forest", "gui/images/message/lore/forest.png");		
		
		if (!isObject($BasilMod::gmannounce::sounds)) {
			$BasilMod::gmannounce::sounds = new ArrayObject();
		}
		$BasilMod::gmannounce::sounds.empty();
		
		%count = getRecordCount(%logos);
		for(%i=0;%i<%count;%i++) {
			%logoName = getRecord(%logos, %i);
			%logoPath = getRecord(%logos, %i+1);
			
			BMGMAnnounceGuiWindow-->logosList.add(%logoName, $BasilMod::gmannounce::logos.count());
			$BasilMod::gmannounce::logos.add(%logoName, %logoPath);
			
			%i++;
		}
		
		%count = getRecordCount(%sounds);
		for(%i=0;%i<%count;%i++) {
			%soundsName = getRecord(%sounds, %i);
			%soundsPath = getRecord(%sounds, %i+1);
			
			BMGMAnnounceGuiWindow-->soundsList.add(%soundsName, $BasilMod::gmannounce::sounds.count());
			$BasilMod::gmannounce::sounds.add(%soundsName, %soundsPath);
			
			%i++;
		}
	}
	
	function BasilMod::netCmd_GMAnnounceShowAnnounce(%this, %config, %message) {
		%image = getRecord(%config, 0);
		%sound = getRecord(%config, 1);
		%message = BasilMod.gmannounce_utf32_collapse(%message);
		
		if (isFile(%sound)) {
			sfxPlayOnce(Audio2D, %sound);
		}
		
		addEventMsgInfoWindow(%message, %image);
		
		cmChat_onServerMessage(%message);
	}
};

if (!isObject(BasilMod) || $BasilMod::pack::version < 4) {
	error("BasilMod::GMAnnounce requires BasilMod::Pack v4+ installed and running!");
} else {
	activatePackage("BasilModGMAnnounce");
	BasilMod::gmannounce_init();
	echo("BasilMod::GMAnnounce v4 loaded.");
}