using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LocalizationManager : MonoBehaviour
{
	public event Action OnChangeLanguageEvent;

	private Dictionary<string, string> localizedText;
	private bool isReady = false;
	private string missingTextString = "Localized text not found";

	public static LocalizationManager Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this);
        }
        else
            Destroy(gameObject);
    }

    public void SetLanguage(string fileName)
	{       
        LoadLocalizedText(fileName);

        //Progress.Instance.Language = fileName;

        OnChangeLanguageEvent?.Invoke();
    }

	public string GetLocalizedValue(string key)
	{
		string result = missingTextString;
		if (localizedText.ContainsKey(key))
		{
			result = localizedText[key];
		}

		return result;
	}

	private void LoadLocalizedText(string fileName)
	{
		localizedText = new Dictionary<string, string>();
		localizedText.Clear();
		switch (fileName)
		{
			case "ru":
                localizedText.Add("level", "Уровень");
                localizedText.Add("loading", "Загрузка...");
                localizedText.Add("costupgrade", "Стоимость:");
                localizedText.Add("unblock", "Разблокировать");
                localizedText.Add("play", "Играть");
                localizedText.Add("back", "Назад");
                localizedText.Add("notenough", "Недостаточно ресурсов");
                localizedText.Add("characterblocked", "Оружие ещё не открыто");
                localizedText.Add("notopenedlevel", "Пройдите предыдущий уровень");
                localizedText.Add("selectlevel", "Выберите уровень");
                localizedText.Add("OK", "OK");               
                localizedText.Add("settings", "Настройки");
                localizedText.Add("continue", "Продолжить");
                localizedText.Add("menu", "Меню");
                localizedText.Add("input", "Управление");
                localizedText.Add("volume", "Громкость");
                localizedText.Add("sound", "Эффекты");
                localizedText.Add("music", "Музыка");            
                localizedText.Add("sensitivity", "Чувствительность");
                localizedText.Add("camerafov", "Обзор камеры");
                localizedText.Add("others", "Другое");
                localizedText.Add("yans", PlayerPrefs.GetString("ANDROID") == "yes" ? "руб" : "ян");
                localizedText.Add("getcoins", "Получено монет");               
                localizedText.Add("finishlevel", "Уровень пройден");
                localizedText.Add("deathplayer", "Поражение");
                localizedText.Add("restart", "Переиграть");
                localizedText.Add("pause", "Пауза");
                localizedText.Add("double", "Удвоить");
                localizedText.Add("respawn", "Продолжить");               
                localizedText.Add("ads", "Реклама");
                localizedText.Add("next", "Далее");
                localizedText.Add("skip", "Пропустить");
                localizedText.Add("select", "Выбрать");
                localizedText.Add("death_player_continue_info", "Нажмите Продолжить, чтобы возвратиться в игру");
                localizedText.Add("dps","Мощь");
                localizedText.Add("level_goals", "Цели уровня:");
                localizedText.Add("all_sound_off", "На iOS звук временно не работает.\r\nПриносим извинения за неудобства.");


                //управление
                localizedText.Add("wasd", "W.A.S.D.");
                localizedText.Add("movement", "Движение");
                localizedText.Add("jump", "Прыжок");
                localizedText.Add("sprint", "Ускорение");
                localizedText.Add("crouch", "Пригнуться");
                localizedText.Add("reload", "Перезарядка");
                localizedText.Add("aim", "Прицелиться");
                localizedText.Add("fire", "Стрельба");

                localizedText.Add("space", "Пробел");
                localizedText.Add("left_shift", "Лев. Shift");
                localizedText.Add("right_mouse", "ПКМ");
                localizedText.Add("left_mouse", "ЛКМ");


                break;
            case "es":
                localizedText.Add("level", "Nivel");
                localizedText.Add("loading", "Carga...");
                localizedText.Add("costupgrade", "Costo:");
                localizedText.Add("unblock", "Desbloquear");
                localizedText.Add("play", "Jugar");
                localizedText.Add("back", "Atrás");
                localizedText.Add("notenough", "Recursos escasos");
                localizedText.Add("characterblocked", "Las armas aún no están abiertas");
                localizedText.Add("notopenedlevel", "Completa el nivel anterior");
                localizedText.Add("selectlevel", "Seleccione un nivel");
                localizedText.Add("OK", "OK");
                localizedText.Add("settings", "Ajustes");
                localizedText.Add("continue", "Continuar");
                localizedText.Add("menu", "Menú");
                localizedText.Add("input", "Gestión");
                localizedText.Add("volume", "Volumen");
                localizedText.Add("sound", "Efectos");
                localizedText.Add("music", "Músico");
                localizedText.Add("sensitivity", "Sensibilidad");
                localizedText.Add("camerafov", "Revisión de la cámara");
                localizedText.Add("others", "Lo otro");
                localizedText.Add("yans", PlayerPrefs.GetString("ANDROID") == "yes" ? "руб" : "ян");
                localizedText.Add("getcoins", "Monedas recibidas");
                localizedText.Add("finishlevel", "Nivel aprobado");
                localizedText.Add("deathplayer", "Derrota");
                localizedText.Add("restart", "Volver a jugar");
                localizedText.Add("pause", "Pausa");
                localizedText.Add("double", "Duplicar");
                localizedText.Add("respawn", "Continuar");
                localizedText.Add("ads", "Publicidad");
                localizedText.Add("next", "Siguiente");
                localizedText.Add("skip", "Omitir");
                localizedText.Add("select", "Elegir");
                localizedText.Add("death_player_continue_info", "Haz clic en Continuar para volver al juego");
                localizedText.Add("dps", "Fuerza");
                localizedText.Add("level_goals", "Objetivos de nivel:");
                localizedText.Add("all_sound_off", "En iOS, el sonido no funciona temporalmente.\r\nPedimos disculpas por las molestias.");


                //управление
                localizedText.Add("wasd", "W.A.S.D.");
                localizedText.Add("movement", "Movimiento");
                localizedText.Add("jump", "Salto");
                localizedText.Add("sprint", "Aceleración");
                localizedText.Add("crouch", "Agacharse");
                localizedText.Add("reload", "Sobrecarga");
                localizedText.Add("aim", "Apuntar");
                localizedText.Add("fire", "Tiroteo");

                localizedText.Add("space", "Espacio");
                localizedText.Add("left_shift", "Izquierdo Shift");
                localizedText.Add("right_mouse", "Botón derecho del ratón");
                localizedText.Add("left_mouse", "Botón izquierdo del ratón");


                break;
            case "de":
                localizedText.Add("level", "Level");
                localizedText.Add("loading", "Laden...");
                localizedText.Add("costupgrade", "Wert:");
                localizedText.Add("unblock", "Freigeben");
                localizedText.Add("play", "Spielen");
                localizedText.Add("back", "Zurück");
                localizedText.Add("notenough", "Nicht genügend Ressourcen");
                localizedText.Add("characterblocked", "Die Waffe ist noch nicht geöffnet");
                localizedText.Add("notopenedlevel", "Schließe die vorherige Level ab");
                localizedText.Add("selectlevel", "Wählen Sie eine Level aus");
                localizedText.Add("OK", "OK");
                localizedText.Add("settings", "Die Einstellungen");
                localizedText.Add("continue", "Fortfahren");
                localizedText.Add("menu", "Das Menü");
                localizedText.Add("input", "Steuerung");
                localizedText.Add("volume", "Lautstärke");
                localizedText.Add("sound", "Effekte");
                localizedText.Add("music", "Die Musik");
                localizedText.Add("sensitivity", "Empfindlichkeit");
                localizedText.Add("camerafov", "Überblick über die Kamera");
                localizedText.Add("others", "Andere");
                localizedText.Add("yans", PlayerPrefs.GetString("ANDROID") == "yes" ? "руб" : "ян");
                localizedText.Add("getcoins", "Münzen erhalten");
                localizedText.Add("finishlevel", "Level bestanden");
                localizedText.Add("deathplayer", "Niederlage");
                localizedText.Add("restart", "Noch einmal spielen");
                localizedText.Add("pause", "Pause");
                localizedText.Add("double", "Verdoppeln");
                localizedText.Add("respawn", "Fortfahren");
                localizedText.Add("ads", "Werbung");
                localizedText.Add("next", "Weiter");
                localizedText.Add("skip", "Verpassen");
                localizedText.Add("select", "Wählen");
                localizedText.Add("death_player_continue_info", "Klicken Sie auf Fortfahren, um zum Spiel zurückzukehren");
                localizedText.Add("dps", "Kraft");
                localizedText.Add("level_goals", "Level-Ziele:");
                localizedText.Add("all_sound_off", "Auf iOS funktioniert der Sound vorübergehend nicht.\r\nWir entschuldigen uns für die Unannehmlichkeiten.");


                //управление
                localizedText.Add("wasd", "W.A.S.D.");
                localizedText.Add("movement", "Der Verkehr");
                localizedText.Add("jump", "Sprung");
                localizedText.Add("sprint", "Beschleunigung");
                localizedText.Add("crouch", "Sich beugen");
                localizedText.Add("reload", "Überladung");
                localizedText.Add("aim", "Zielen");
                localizedText.Add("fire", "Schießen");

                localizedText.Add("space", "Lücke");
                localizedText.Add("left_shift", "Linke Shift");
                localizedText.Add("right_mouse", "Rechte Maustaste");
                localizedText.Add("left_mouse", "Linke Maustaste");


                break;
            case "en":
                localizedText.Add("level", "Level");
                localizedText.Add("platform", "You are playing");
                localizedText.Add("computer", "On a computer");
                localizedText.Add("phone", "On a phone");
				localizedText.Add("loading", "Loading...");
                localizedText.Add("block", "Blocked");
                localizedText.Add("coinsforkill", "Coins for kill");
                localizedText.Add("damagebody", "Damage:");
                localizedText.Add("damagehead", "Head damage:");
                localizedText.Add("health", "Health:");
                localizedText.Add("playerlevel", "Level:");
                localizedText.Add("upgrade", "Upgrade");
                localizedText.Add("costupgrade", "Cost:");
                localizedText.Add("unblock", "Unblock");
                localizedText.Add("map", "Map:");
                localizedText.Add("type", "Type:");
                localizedText.Add("firerate", "Fire rate");
                localizedText.Add("magazine", "Magazine");
                localizedText.Add("speed", "Speed");
                localizedText.Add("slow", "Slow");
                localizedText.Add("medium", "Medium");
                localizedText.Add("high", "High");
                localizedText.Add("play", "Play");
                localizedText.Add("back", "Back");
                localizedText.Add("notenough", "Not enough resources");
                localizedText.Add("characterblocked", "The weapon has not been opened yet");
                localizedText.Add("notopenedlevel", "Complete the previous level");
                localizedText.Add("selectlevel", "Select a level");
                localizedText.Add("shop", "Shop");
                localizedText.Add("OK", "OK");
                localizedText.Add("settings", "Settings");
                localizedText.Add("continue", "Continue");
                localizedText.Add("menu", "Menu");
                localizedText.Add("input", "Input");
                localizedText.Add("reset", "Reset");
                localizedText.Add("volume", "Volume");
                localizedText.Add("sound", "VFX");
                localizedText.Add("music", "Music");
                localizedText.Add("sensitivity", "Sensitivity");
                localizedText.Add("camerafov", "Camera FOV");
                localizedText.Add("others", "Others");
                localizedText.Add("yans", PlayerPrefs.GetString("ANDROID") == "yes" ? "rub" : "yans");
                localizedText.Add("fast", "Fast");
                localizedText.Add("story", "Story");
                localizedText.Add("develop", "Develop");
                localizedText.Add("getcoins", "Coins received");
                localizedText.Add("getcrystals", "Crystals received");
                localizedText.Add("killedenemies", "Enemies killed");
                localizedText.Add("finishlevel", "The level is completed");
                localizedText.Add("deathplayer", "Defeat");
                localizedText.Add("restart", "Replay");
                localizedText.Add("pause", "Pause");
                localizedText.Add("double", "Double");
                localizedText.Add("respawn", "Respawn");
                localizedText.Add("ads", "Ads");
                localizedText.Add("next", "Next");
                localizedText.Add("skip", "Skip");
                localizedText.Add("select", "Select");
                localizedText.Add("recommend", "Recommended\r\n hero:");
                localizedText.Add("damagebooster", "Super damage");
                localizedText.Add("damageheadbooster", "Super head damage");
                localizedText.Add("reloadbooster", "Fast reload");
                localizedText.Add("fireratebooster", "Super firerate");
                localizedText.Add("wave", "Wave");
                localizedText.Add("grenade_tutorial_pc", "The \"G\" key is a grenade throw");
                localizedText.Add("grenade_tutorial_mobile", "Button       is a grenade throw");
                localizedText.Add("shooting_tutorial_mobile", "The button       is to turn on auto-shooting");
                localizedText.Add("barell_tutorial", "Shoot the barrel");
                localizedText.Add("instakill", "INSTANT KILL");
                localizedText.Add("death_player_continue_info", "Click Continue to return to the game");
                localizedText.Add("visit", "To visit: ");
                localizedText.Add("lobby", "Lobby");
                localizedText.Add("continue_move", "Kill'em all!");
                localizedText.Add("booster_tutorial", "Take the booster!");
                localizedText.Add("holding_tutorial", "In <color=#AAE3FF>\"Holding\"</color> mode the hero cannot move");
                localizedText.Add("no_purchase", "The purchase has not been completed");
                localizedText.Add("dps", "Power");
                localizedText.Add("level_goals", "Level goals:");
                localizedText.Add("all_sound_off", "The sound temporarily does not work on iOS.\r\nWe apologize for the inconvenience.");

                //подсказки
                localizedText.Add("loading_01_desc_01", "High Damage Area");
                localizedText.Add("loading_01_desc_02", "Normal Damage Area");
                localizedText.Add("hint", "HINT");
                localizedText.Add("hint_01_desc_01", "A headshot does more damage");
                localizedText.Add("loading_02_desc_01", "Explosive barrels");
                localizedText.Add("hint_02_desc_01", "Try to lure the enemy to such a barrel, and then shoot at it");
                localizedText.Add("loading_03_desc_01",
                    "<color=#EE6262>95</color> - hit in the head\r\n" +
                    "<color=#FFFFFF>46</color> - normal damage\r\n" +
                    "<color=#FA9E4D>129</color> - explosion damage");
                localizedText.Add("hint_03_desc_01", "Watch how your weapon deals damage");
                localizedText.Add("hint_04_desc_01", "Upgrade by collecting <color=#88FF3B>energy</color> from killed enemies in <color=#FFCD34>\"Survive\"</color> mode");
                localizedText.Add("hint_05_desc_01", "You get points by killing undead in <color=#FFCD34>\"Survive\"</color> mode");

                //выжить
                localizedText.Add("wave_complete", "The wave is completed");
                localizedText.Add("select_perks", "Choose a perk");
                localizedText.Add("survive_mode_current_wave", "You got ");
                localizedText.Add("survive_mode_record", "Record: ");
                localizedText.Add("survive_mode_new_record", "A new record: ");

                //перки
                localizedText.Add("Increase_Damage_Perk", "Increase all <color=#FF503B>damage</color> by <color=#AAE3FF>12%</color> of the base value");
                localizedText.Add("Increase_Damage_Head_Perk", "Increase <color=#FF503B>head damage</color> by <color=#AAE3FF>12%</color> of the base value");
                localizedText.Add("Increase_Fire_Rate_Perk", "Increase <color=#FFBB3B>firerate</color> by <color=#AAE3FF>5%</color> of the base value");
                localizedText.Add("Increase_Health_Perk", "Increase <color=#88FF3B>health</color> by <color=#AAE3FF>20%</color> of the base value");
                localizedText.Add("Increase_Reload_Speed_Perk", "Increase <color=#FFBB3B>reload rate</color> by <color=#AAE3FF>8%</color> of the base value");
                localizedText.Add("Increase_Instakill_Chance_Perk", "Increase/add <color=#FF69F1>the chance of instant kill</color> by <color=#AAE3FF>1%</color>, but not more than <color=#AAE3FF>30%</color>");
                localizedText.Add("Increase_Double_Damage_Chance_Perk", "Increase/add <color=#FF69F1>the chance to deal double damage</color> by <color=#AAE3FF>1%</color>, but not more than <color=#AAE3FF>40%</color>");
                localizedText.Add("Increase_Speed_Perk", "Increase hero's <color=#FFBB3B>movement speed by <color=#AAE3FF>3%</color> of the base value");
                localizedText.Add("Heal_Perk", "<color=#88FF3B>Heal</color> <color=#AAE3FF>30%</color> of maximum health");
                localizedText.Add("Baton_Extra_Weapon_Perk", "Gives you <color=#88FF3B>Explosive Axe</color> for <color=#AAE3FF>15 sec.</color>\r\n Base damage: <color=#AAE3FF>240</color>\r\nCan't hurt you\r\nAim and throw");
                localizedText.Add("Current_Value", "Current value: ");
                localizedText.Add("Get_One_Hit_Shield_Perk", "Gives you a <color=#88FF3B>Shield</color> that protects you from <color=#AAE3FF>1</color> hit");
                localizedText.Add("Missile_Auto_Weapon_Perk", "Activates <color=#88FF3B>Missile Strike</color> on enemies every <color=#AAE3FF>5 - 12 sec.</color>\r\nBase damage: <color=#AAE3FF>240</color>\r\nCan't hurt you");

                //названия уровней
                localizedText.Add("park", "Park");
                //типы уровней
                localizedText.Add("slaughter", "Slaughter");
                localizedText.Add("survive", "Survive");
                localizedText.Add("extraction", "Evacuation");
                localizedText.Add("holding", "Holding");
                localizedText.Add("clearing", "Clearing");
                localizedText.Add("boss", "Boss");

                //персонажи
                localizedText.Add("rifleman_ability", "Dynamite");
                localizedText.Add("shotgunman_ability", "Sticky grenades");
                localizedText.Add("sniperman_ability", "Triple head damage");
                localizedText.Add("batonman_ability", "Explosive axe");
                localizedText.Add("pistolman_ability", "A pistol with a silencer");
                localizedText.Add("revolverman_ability", "5% chance of instant kill");

                localizedText.Add("rifleman_name", "Saboteur");
                localizedText.Add("shotgunman_name", "Commando");
                localizedText.Add("sniperman_name", "Dead Eye");
                localizedText.Add("batonman_name", "Pyro");
                localizedText.Add("pistolman_name", "Killer");
                localizedText.Add("revolverman_name", "Biker");

                //tutorial
                localizedText.Add("tutorial_step_one_01", "Press");
                localizedText.Add("tutorial_step_one_02", "Press");
                localizedText.Add("tutorial_step_two_01", "Each hero has an <color=#AAE3FF>individual</color> weapon");
                localizedText.Add("tutorial_step_two_02", "Press");
                localizedText.Add("tutorial_step_three_01", "<color=#50D500>Green</color> numbers are an increase in the parameter at the next level");
                localizedText.Add("tutorial_step_three_02", "Each character has an <color=#AAE3FF>unique</color> ability");
                localizedText.Add("tutorial_step_three_03", "In the <color=#FFCD34>\"Survive\"</color> mode upgrade your hero by collecting <color=#50D500>green</color> energy from killed enemies");
                localizedText.Add("tutorial_step_three_04", "Earn points and set records");
                localizedText.Add("tutorial_step_four_01", "The game gives you the hero to complete");

                //управление
                localizedText.Add("wasd", "W.A.S.D.");
                localizedText.Add("movement", "Movement");
                localizedText.Add("jump", "Jump");
                localizedText.Add("sprint", "Sprint");
                localizedText.Add("crouch", "Crouch");
                localizedText.Add("throwgrenade", "Throw grenade");
                localizedText.Add("meleeattack", "Melee attack");
                localizedText.Add("reload", "Reload");
                localizedText.Add("aim", "Aim");
                localizedText.Add("fire", "Fire");
                localizedText.Add("flashlight", "Flashlight");
                localizedText.Add("interact", "Interact");

                localizedText.Add("space", "Space");
                localizedText.Add("left_shift", "Left Shift");
                localizedText.Add("right_mouse", "Right Mouse");
                localizedText.Add("left_mouse", "Left Mouse");

                break;
            case "tr":
                localizedText.Add("level", "Seviye");
                localizedText.Add("platform", "Oynuyorsunuz");
                localizedText.Add("computer", "Bilgisayarda");
                localizedText.Add("phone", "Telefonda");
                localizedText.Add("loading", "indiriyor...");
                localizedText.Add("block", "Engellendi");
                localizedText.Add("coinsforkill", "Cinayetten para");
                localizedText.Add("damagebody", "Zarar:");
                localizedText.Add("damagehead", "Kafaya hasar:");
                localizedText.Add("health", "Sağlık:");
                localizedText.Add("playerlevel", "Düzey:");
                localizedText.Add("upgrade", "Yükseltme");
                localizedText.Add("costupgrade", "Fiyat:");
                localizedText.Add("unblock", "Kilidini aç");
                localizedText.Add("map", "Harita:");
                localizedText.Add("type", "Tür:");
                localizedText.Add("firerate", "Ateş");
                localizedText.Add("magazine", "Şarjör");
                localizedText.Add("speed", "Hız");
                localizedText.Add("slow", "Yavaş");
                localizedText.Add("medium", "Orta");
                localizedText.Add("high", "Yüksek");
                localizedText.Add("play", "Oyna");
                localizedText.Add("back", "Geri");
                localizedText.Add("notenough", "Yeterli kaynak yok");
                localizedText.Add("characterblocked", "Silah henüz açılmadı");
                localizedText.Add("notopenedlevel", "Bir önceki seviyeyi tamamlayın");
                localizedText.Add("selectlevel", "Seviyeyi seçin");
                localizedText.Add("shop", "Mağaza");
                localizedText.Add("OK", "OK");
                localizedText.Add("settings", "Ayarlar");
                localizedText.Add("continue", "Devam etmek");
                localizedText.Add("menu", "Menü");
                localizedText.Add("input", "Yönetim");
                localizedText.Add("reset", "Giriş");
                localizedText.Add("volume", "Ses");
                localizedText.Add("sound", "VFX");
                localizedText.Add("music", "Müzik");
                localizedText.Add("sensitivity", "Duyarlılık");
                localizedText.Add("camerafov", "Kamera FOV");
                localizedText.Add("others", "Diğerleri");
                localizedText.Add("yans", PlayerPrefs.GetString("ANDROID") == "yes" ? "rub" : "yans");
                localizedText.Add("fast", "Hızlı");
                localizedText.Add("story", "Hikâye");
                localizedText.Add("develop", "Geliştirmek");
                localizedText.Add("getcoins", "Alınan paralar");
                localizedText.Add("getcrystals", "Alınan kristaller");
                localizedText.Add("killedenemies", "Düşmanlar öldürüldü");
                localizedText.Add("finishlevel", "Seviye geçti");
                localizedText.Add("deathplayer", "Yenilgi");
                localizedText.Add("restart", "Tekrar Oynat");
                localizedText.Add("pause", "Duraklat");
                localizedText.Add("double", "İkiye katlamak");
                localizedText.Add("respawn", "Yeniden doğmak");
                localizedText.Add("ads", "Reklamlar");
                localizedText.Add("next", "Sonraki");
                localizedText.Add("skip", "Atlamak");
                localizedText.Add("select", "Seçmek");
                localizedText.Add("recommend", "Önerilen\r\n kahraman:");
                localizedText.Add("damagebooster", "Süper hasar");
                localizedText.Add("damageheadbooster", "Süper kafa hasarı");
                localizedText.Add("reloadbooster", "Hızlı şarj");
                localizedText.Add("fireratebooster", "Süper ateş hızı");
                localizedText.Add("wave", "Dalga");
                localizedText.Add("grenade_tutorial_pc", "\"G\" tuşu bir el bombası atışıdır");
                localizedText.Add("grenade_tutorial_mobile", "Düğme       bir el bombası atışı");
                localizedText.Add("shooting_tutorial_mobile", "Düğme     otomatik çekimi açmaktır");
                localizedText.Add("barell_tutorial", "Namluyu vur");
                localizedText.Add("instakill", "Anında ÖLDÜRME");
                localizedText.Add("death_player_continue_info", "Oyuna dönmek için Devam'a tıklayın");
                localizedText.Add("visit", "Ziyaret etmek için: ");
                localizedText.Add("lobby", "Lobi");
                localizedText.Add("continue_move", "Hepsini öldürün!");
                localizedText.Add("booster_tutorial", "Güçlendiriciyi al!");
                localizedText.Add("holding_tutorial", "<color=#AAE3FF>\"Tutma\"</color> modu kahraman hareket edemez");
                localizedText.Add("no_purchase", "Satın alma işlemi tamamlanmadı");
                localizedText.Add("dps", "Güç");
                localizedText.Add("level_goals", "Seviyedeki hedefler:");
                localizedText.Add("all_sound_off", "iOS'ta ses geçici olarak çalışmıyor.\r\nRahatsızlıktan dolayı özür dileriz.");

                //подсказки
                localizedText.Add("loading_01_desc_01", "Yüksek Hasar Alanı");
                localizedText.Add("loading_01_desc_02", "Normal Hasar Alanı");
                localizedText.Add("hint", "ipucu");
                localizedText.Add("hint_01_desc_01", "Bir headshot daha fazla zarar verir");
                localizedText.Add("loading_02_desc_01", "Patlayıcı variller");
                localizedText.Add("hint_02_desc_01", "Düşmanı böyle bir namluya çekmeye çalışın ve sonra ona ateş edin");
                localizedText.Add("loading_03_desc_01",
                    "<color=#EE6262>95</color> - kafasına vurmak\r\n" +
                    "<color=#FFFFFF>46</color> - normal hasar\r\n" +
                    "<color=#FA9E4D>129</color> - patlama hasarı");
                localizedText.Add("hint_03_desc_01", "Silahınızın nasıl hasar verdiğini izleyin");
                localizedText.Add("hint_04_desc_01", "<color=#FFCD34>\"Dayanmak\"</color> modunda öldürülen düşmanlardan <color=#88FF3B>enerji</color> toplayarak yükseltin");
                localizedText.Add("hint_05_desc_01", "<color=#FFCD34>\"Dayanmak\"</color> modunda ölümsüzleri öldürerek puan kazanırsınız");

                //выжить
                localizedText.Add("wave_complete", "Dalga tamamlandı");
                localizedText.Add("select_perks", "Bir avantaj seçin");
                localizedText.Add("survive_mode_current_wave", "Sende var ");
                localizedText.Add("survive_mode_record", "Rekor: ");
                localizedText.Add("survive_mode_new_record", "Yeni bir rekor: ");

                //перки
                localizedText.Add("Increase_Damage_Perk", "Tüm <color=#FF503B>hasarını</color> temel değerin <color=#AAE3FF>%12</color> oranında artırın");
                localizedText.Add("Increase_Damage_Head_Perk", "<color=#FF503B>Kafa hasarını</color> temel değerin <color=#AAE3FF>%12</color> artırın");
                localizedText.Add("Increase_Fire_Rate_Perk", "<color=#FFBB3B>Ateş oranını</color> temel değerin <color=#AAE3FF>%5</color> artırın");
                localizedText.Add("Increase_Health_Perk", "<color=#88FF3B>Sağlığını</color> temel değerin <color=#AAE3FF>%20</color> oranında artırın");
                localizedText.Add("Increase_Reload_Speed_Perk", "<color=#FFBB3B>Yeniden yükleme oranını</color> temel değerin <color=#AAE3FF>%8</color> oranında artırın");
                localizedText.Add("Increase_Instakill_Chance_Perk", "<color=#FF69F1>Anında öldürme şansını</color> <color=#AAE3FF>%1</color> oranında artırın/ekleyin, ancak <color=#AAE3FF>%30</color>'dan fazla değil");
                localizedText.Add("Increase_Double_Damage_Chance_Perk", "<color=#FF69F1>Çift hasar verme şansını</color> <color=#AAE3FF>%1</color> oranında artırın/ekleyin, ancak <color=#AAE3FF>%40</color>'dan fazla değil");
                localizedText.Add("Increase_Speed_Perk", "Kahramanın <color=#FFBB3B>hareket hızını temel değerin <color=#AAE3FF>%3</color> artırın");
                localizedText.Add("Heal_Perk", "<color=#88FF3B>İyileştir</color> maksimum sağlığın <color=#AAE3FF>%30</color>'u");
                localizedText.Add("Baton_Extra_Weapon_Perk", "Size <color=#88FF3B>Patlayıcı balta</color> verir <color=#AAE3FF>15 sn.</color>\r\n Taban hasarı: <color=#AAE3FF>240</color>\r\nSana zarar veremem\r\nNişan al ve fırlat");
                localizedText.Add("Current_Value", "Geçerli değer: ");
                localizedText.Add("Get_One_Hit_Shield_Perk", "Size <color=#AAE3FF>1</color> etkisinden koruyan bir <color=#88FF3B>Kalkan</color> sağlar");
                localizedText.Add("Missile_Auto_Weapon_Perk", "Her <color=#AAE3FF>5 - 12 sn.</color> bir düşmanlara <color=#88FF3B>füze saldırısını</color> etkinleştirir.\r\nTaban hasarı: <color=#AAE3FF>240</color>\r\nSana zarar veremem");

                //названия уровней
                localizedText.Add("park", "Park");
                //типы уровней
                localizedText.Add("slaughter", "Katliam");
                localizedText.Add("survive", "Dayanmak");
                localizedText.Add("extraction", "Tahliye");
                localizedText.Add("holding", "Tutma");
                localizedText.Add("clearing", "Temizleme");
                localizedText.Add("boss", "Boss");

                //персонажи
                localizedText.Add("rifleman_ability", "Dinamit");
                localizedText.Add("shotgunman_ability", "Yapışkan bombalar");
                localizedText.Add("sniperman_ability", "Üçlü kafa hasarı");
                localizedText.Add("batonman_ability", "Patlayıcı balta");
                localizedText.Add("pistolman_ability", "Susturuculu bir tabanca");
                localizedText.Add("revolverman_ability", "Anında öldürme şansı %5");

                localizedText.Add("rifleman_name", "Sabotajcı");
                localizedText.Add("shotgunman_name", "Komando");
                localizedText.Add("sniperman_name", "Ölü Göz");
                localizedText.Add("batonman_name", "Piro");
                localizedText.Add("pistolman_name", "Katil");
                localizedText.Add("revolverman_name", "Motorcu");

                //tutorial
                localizedText.Add("tutorial_step_one_01", "Basın");
                localizedText.Add("tutorial_step_one_02", "Basın");
                localizedText.Add("tutorial_step_two_01", "Her kahramanın bir <color=#AAE3FF>bireysel</color> silahı vardır");
                localizedText.Add("tutorial_step_two_02", "Basın");
                localizedText.Add("tutorial_step_three_01", "<color=#50D500>Yeşil</color> sayılar bir sonraki seviyedeki parametrede bir artıştır");
                localizedText.Add("tutorial_step_three_02", "Her karakterin bir <color=#AAE3FF>benzersiz</color> yeteneği vardır");
                localizedText.Add("tutorial_step_three_03", "<color=#FFCD34>\"Dayanmak\"</color> modunda, öldürülen düşmanlardan <color=#50D500>yeşil</color> enerji toplayarak kahramanınızı yükseltin");
                localizedText.Add("tutorial_step_three_04", "Puan kazanın ve rekorlar kırın");
                localizedText.Add("tutorial_step_four_01", "Oyun size tamamlamak için kahraman verir");

                //управление
                localizedText.Add("wasd", "W.A.S.D.");
                localizedText.Add("movement", "Hareket");
                localizedText.Add("jump", "Atlama");
                localizedText.Add("sprint", "Hızlı koşma");
                localizedText.Add("crouch", "Çömelme");
                localizedText.Add("throwgrenade", "El bombası atmak");
                localizedText.Add("meleeattack", "Yakın dövüş saldırısı");
                localizedText.Add("reload", "Yeniden yükle");
                localizedText.Add("aim", "Amaç");
                localizedText.Add("fire", "Ateş");
                localizedText.Add("flashlight", "El feneri");
                localizedText.Add("interact", "Etkileşim");

                localizedText.Add("space", "Boşluk");
                localizedText.Add("left_shift", "Sol Shift");
                localizedText.Add("right_mouse", "Sağ Fare");
                localizedText.Add("left_mouse", "Sol Fare");

                break;
        }		
	}

	public bool GetIsReady()
	{
		return isReady;
	}

}

[System.Serializable]
public class LocalizationData
{
	public LocalizationItem[] items;

}

[System.Serializable]
public class LocalizationItem
{
	public string key;
	public string value;
}