namespace SunamoWinStd._public.SunamoEnums.Enums;

// Poradi 1-30 odpovida prvnimu radku ouska "Mail accounts" v
// https://docs.google.com/spreadsheets/d/1IaYPQoAFwJ0J5eB-8Bn6gyABE2PCl1nRyQDjGXl3-l0 (gid=144466497).
// PaleMoon a EdgeStable v tomto radku nejsou, proto jsou zarazeny az za nej.
public enum Browsers : byte
{
    Chrome = 1,
    Firefox = 2,
    ChromeStable = 3,
    Opera = 4,
    Vivaldi = 5,
    Slimjet = 6,
    EdgeBeta = 7,
    EdgeDev = 8,
    EdgeCanary = 9,
    Tor = 10,
    Bravebrowser = 11,
    NawerWhale = 12,
    LibreWolf = 13,
    OperaGX = 14,
    Min = 15,
    Basilisk = 16,
    ChromeBeta = 17,
    ChromeDev = 18,
    ChromeCanary = 19,
    KMeleon = 20,
    Comet = 21,
    Arc = 22,
    Zen = 23,
    AvastBrowser = 24,
    Comodo = 25,
    Mullvad = 26,
    Floorp = 27,
    Sidekick = 28,
    Midori = 29,
    WaterFox = 30,
    PaleMoon = 31,
    EdgeStable = 32,
    None = 255
}
