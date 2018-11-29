using System;
using System.Collections.Generic;
using Jypeli;
using Jypeli.Assets;
using Jypeli.Controls;
using Jypeli.Widgets;

public class Harjoitustyo2 : PhysicsGame
{
    // TODO: silmukka Samuli: ks. demo 6, tehtävä Tauno 2.4
    // TODO: silmukka Aleksi: ks. demo 7, tehtävä Tauno 1B

    private const double nopeus = 200;
    private const double hyppyNopeus = 330;
    private const int RUUDUN_KOKO = 40;
    private const int TASON_KOKO = 20;
    private int kenttaNro = 1;
    List<Label> valikonKohdat;



    private IntMeter pisteLaskuri;
    private PlatformCharacter pelaaja1;


    private Image pelaajanKuva = LoadImage("Pukki");
    private Image pulloKuva = LoadImage("beer1");
    private Image maaliKuva = LoadImage("Talo");
    private Image vihunKuva = LoadImage("santapaha");

    // SoundEffect maaliAani = LoadSoundEffect("maali");

    /// <summary> 
    /// Pistää pelin käyntiin kutsumalla tarvittavat aliohjelmat
    /// </summary>
    /// <param name=" ">kentan numeroa vastaava kentta</param>
    /// <returns> </returns>

    public override void Begin()
    {
        // LuoKentta(mikaKentta);
        SeuraavaKentta();
        Camera.ZoomToLevel();
        // LuoKentta(mikaKentta);
        // LisaaNappaimet();


        // MediaPlayer.Play("taustamusa");

        Camera.Follow(pelaaja1);
        Camera.StayInLevel = true;
        PhoneBackButton.Listen(ConfirmExit, "Lopeta peli");
        Keyboard.Listen(Key.Escape, ButtonState.Pressed, ConfirmExit, "Lopeta peli");

    }

    /// <summary>
    /// Pelin aloitusvalikon aliohjelma
    /// </summary>
    /// <param name="valikonkohdat">Alkuvalikon valikot</param>
    /// <returns>Alkuvalikon valikot</returns>


    private void Valikko()
    {
        ClearAll();                                       // Tyhjennetään kenttä kaikista peliolioista
        valikonKohdat = new List<Label>();                  // Alustetaan lista, johon valikon kohdat tulevat
        Label kohta1 = new Label("Aloita uusi peli");       // Luodaan uusi Label-olio, joka toimii uuden pelin aloituskohtana
        kohta1.Position = new Vector(0, 40);                // Asetetaan valikon ensimmäinen kohta hieman kentän keskikohdan yläpuolelle
        valikonKohdat.Add(kohta1);                          // Lisätään luotu valikon kohta listaan jossa kohtia säilytetään
        Label kohta2 = new Label("Lopeta peli");
        kohta2.Position = new Vector(0, 0);
        valikonKohdat.Add(kohta2);


        foreach (Label valikonKohta in valikonKohdat)        // Lisätään kaikki luodut kohdat peliin foreach-silmukalla
        {
            Add(valikonKohta);
        }
        Mouse.ListenOn(kohta1, MouseButton.Left, ButtonState.Pressed, SeuraavaKentta, null);   /// Hiiren näppäinkuunnellin
        Mouse.ListenOn(kohta2, MouseButton.Left, ButtonState.Pressed, Exit, null);             /// Hiiren näppäinkuunnellin  
        Mouse.ListenMovement(1.0, ValikossaLiikkuminen, null);                                 /// Hiiren liikkentunnistin


    }
    /// <summary>
    /// Aliohjelma joka tarkastelee hiiren liikettä valikossa
    /// </summary>
    /// <param name="ValokossaLiikkuminen">Valikkoa tarkasteleva aliohjelma</param>
    /// <returns> Aliohjelman mikä tarkkailee hiiren liikettä valikossa</returns>
    private void ValikossaLiikkuminen()
    {
        foreach (Label kohta in valikonKohdat)
        {
            if (Mouse.IsCursorOn(kohta))
            {
                kohta.TextColor = Color.Red;
            }
            else
            {
                kohta.TextColor = Color.Black;
            }

        }
    }



    /// <summary> 
    /// Luodaan kentta järjestyksen mukaisesti
    /// </summary>
    /// <param name="mikaKentta">kentan numeroa vastaava kentta</param>
    /// <returns>kentän numeroa vastaavan kentän</returns>
    private void LuoKentta(char[,] mikaKentta)
    {
        ClearAll();
        Gravity = new Vector(0, -666);
        TileMap kentta = new TileMap(mikaKentta);
        kentta['='] = LisaaTaso;
        kentta['*'] = LisaaPullo;
        kentta['N'] = LisaaPelaaja;
        kentta['M'] = LisaaMaali;
        kentta.Insert(RUUDUN_KOKO, RUUDUN_KOKO);
        Level.CreateBorders();
        Level.Background.CreateGradient(Color.White, Color.Pink);


    }




    /// <summary> 
    /// Seuraavaan kenttään siirtävä aliohjelma
    /// </summary>
    /// <param name=" "> </param>
    /// <returns> </returns>

    private void SeuraavaKentta()
    {
        Valikko();
        ClearAll();

        if (kenttaNro == 1) LuoKentta(Kentta1);
        else if (kenttaNro == 2) LuoKentta(Kentta2);
        // else if (kenttaNro == 3) LuoKentta();
        else if (kenttaNro > 2) Exit();

        LisaaNappaimet();
        LuoPistelaskuri();
        LuoVihollinen();


    }


    /// <summary> 
    /// Näppäinkomennot peliin luova aliohjelma
    /// </summary>
    /// <param name=" ">kentän numero</param>
    /// <returns> </returns>
    private void LisaaNappaimet()
    {
        Keyboard.Listen(Key.F1, ButtonState.Pressed, ShowControlHelp, "Näytä ohjeet");
        Keyboard.Listen(Key.Escape, ButtonState.Pressed, Exit, "Poistu pelistä");

        Keyboard.Listen(Key.Left, ButtonState.Down, Liikuta, "Liikkuu vasemmalle", pelaaja1, -nopeus);
        Keyboard.Listen(Key.Right, ButtonState.Down, Liikuta, "Liikkuu oikealle", pelaaja1, nopeus);
        Keyboard.Listen(Key.Up, ButtonState.Pressed, Hyppaa, "Pelaaja hyppää", pelaaja1, hyppyNopeus);


        /// x-boxin ohjaimen näppäinkomennot
        /* ControllerOne.Listen(Button.Back, ButtonState.Pressed, Exit, "Poistu pelistä");
        ControllerOne.Listen(Button.DPadLeft, ButtonState.Down, Liikuta, "Pelaaja liikkuu vasemmalle", pelaaja1, -nopeus);
        ControllerOne.Listen(Button.DPadRight, ButtonState.Down, Liikuta, "Pelaaja liikkuu oikealle", pelaaja1, nopeus);
        ControllerOne.Listen(Button.A, ButtonState.Pressed, Hyppaa, "Pelaaja hyppää", pelaaja1, hyppyNopeus); */
    }

    /// <summary> 
    /// Aliohjelma joka luo peliin tason
    /// </summary>
    /// <param name=" "></param>
    /// <returns>tason</returns>

    /*private PhysicsObject valikonKohdat()
    {
        PhysicsObject valikko = PhysicsObject.CreateStaticObject(40, 40);
        valikko.Color = Color.Red;
        valikko.Shape = Shape.Rectangle;
        return valikko;
    }*/


    private PhysicsObject LisaaTaso()
    {
        PhysicsObject taso = PhysicsObject.CreateStaticObject(TASON_KOKO, TASON_KOKO);
        taso.Color = Color.Red;
        taso.Shape = Shape.Rectangle;
        return taso;
    }

    /// <summary> 
    /// Aliohjelma joka luo peliin pullon
    /// </summary>
    /// <param name=" "></param>
    /// <returns>pullon</returns>
    private PhysicsObject LisaaPullo()
    {
        PhysicsObject pullo = PhysicsObject.CreateStaticObject(5, 10);
        pullo.Image = pulloKuva;
        pullo.Tag = "pullo";
        return pullo;
    }

    /// <summary> 
    /// Aliohjelma joka luo peliin maalin
    /// </summary>
    /// <param name=" "></param>
    /// <returns>maalin</returns>
    private PhysicsObject LisaaMaali()
    {
        PhysicsObject maali = PhysicsObject.CreateStaticObject(20, 20);
        maali.Image = maaliKuva;
        maali.Tag = "maali";
        return maali;
    }

    /// <summary> 
    /// Aliohjelma joka luo peliin pelaajan
    /// </summary>
    /// <param name=" "></param>
    /// <returns>pelaajan</returns>
    private PlatformCharacter LisaaPelaaja()
    {
        pelaaja1 = new PlatformCharacter(20, 20);
        pelaaja1.Mass = 4.0;
        pelaaja1.Image = pelaajanKuva;
        AddCollisionHandler(pelaaja1, OsuPulloon);
        AddCollisionHandler(pelaaja1, OsuMaaliin);
        AddCollisionHandler(pelaaja1, OsuVihuun);

        return pelaaja1;
    }

    /// <summary> 
    /// Aliohjelma joka liikuttaa hahmoa tietyllä nopeudella
    /// </summary>
    /// <param name="hahmo"></param>
    /// <param name="nopeus"></param>
    /// <returns> </returns>
    private void Liikuta(PlatformCharacter hahmo, double nopeus)
    {
        hahmo.Walk(nopeus);
    }

    /// <summary> 
    /// Aliohjelma joka hypäyttää hahmon tietyllä nopeudella
    /// </summary>
    /// <param name="hahmo"></param>
    /// <param name="nopeus"></param>
    /// <returns> </returns>
    private void Hyppaa(PlatformCharacter hahmo, double nopeus)
    {
        hahmo.Jump(nopeus);
    }

    /// <summary> 
    /// Aliohjelma joka tuhoaa pullon ja antaa siitä pisteen osuttaessa
    /// </summary>
    /// <param name="hahmo"></param>
    /// <param name="kohde"></param>
    /// <returns> </returns>
    private void OsuPulloon(PhysicsObject hahmo, PhysicsObject kohde)
    {
        if (kohde.Tag == "pullo")
        {
            // maaliAani.Play();  TODO: tähän joku ääni jos haluaa osumasta semmosen
            //MessageDisplay.Add("Nastrovja!");
            kohde.Destroy();
            pisteLaskuri.Value += 1;

        }
    }

    /// <summary> 
    /// Aliohjelma joka vaihtaa kentän numeroa maaliin osuttaessa
    /// </summary>
    /// <param name="hahmo"></param>
    /// <param name="kohde"></param>
    /// <returns> </returns>
    private void OsuMaaliin(PhysicsObject hahmo, PhysicsObject kohde)
    {

        if ((kohde.Tag == "maali") && (pisteLaskuri.Value >= 3))
        {
            // maaliAani.Play();  TODO: tähän joku ääni jos haluaa osumasta semmosen
            // MessageDisplay.Add("Hienoa, olet maalissa!");
            kenttaNro++;
            SeuraavaKentta();

        }
    }

    /// <summary> 
    /// Aliohjelma joka keskeyttää pelin hahmon osuessa vihuun
    /// </summary>
    /// <param name="hahmo"></param>
    /// <param name="kohde"></param>
    /// <returns> </returns>
    private void OsuVihuun(PhysicsObject hahmo, PhysicsObject kohde)
    {
        if (kohde.Tag == "vihollinen")
        {
            // maaliAani.Play();  TODO: tähän joku ääni jos haluaa osumasta semmosen
            hahmo.Destroy();
            Exit();

        }
    }

    /// <summary> 
    /// Aliohjelma joka lisää peliin pistelaskurin
    /// </summary>
    /// <param name=""></param>
    /// <returns>pistelaskurin</returns>
    private void LuoPistelaskuri()
    {

        pisteLaskuri = new IntMeter(0);

        Label pisteNaytto = new Label();
        pisteNaytto.X = Screen.Left + 100;
        pisteNaytto.Y = Screen.Top - 100;
        pisteNaytto.TextColor = Color.Black;
        pisteNaytto.Color = Color.White;
        pisteNaytto.Title = "Märgäää";

        pisteNaytto.BindTo(pisteLaskuri);
        Add(pisteNaytto);
    }

    /// <summary> 
    /// Aliohjelma joka lisää peliin vihollisen joka syntyy tietyn ajan välein randomiin paikkaan
    /// </summary>
    /// <param name=" "></param>
    /// <returns> </returns>
    private void LuoVihollinen()
    {
        PhysicsObject vihollinen = new PhysicsObject(25, 25);
        vihollinen.Image = vihunKuva;
        vihollinen.Tag = "vihollinen";
        Add(vihollinen);

        Timer ajastin = new Timer();
        ajastin.Interval = 3.5; // Kuinka usein ajastin "laukeaa" sekunneissa
        ajastin.Timeout += delegate { VihollinenTippuu(vihollinen); }; // Aliohjelma, jota kutsutaan 3.5 sekunnin välein
        ajastin.Start(); 
    }

    /// <summary> 
    /// Aliohjelma joka luo vihollisen randomiin paikkaan
    /// </summary>
    /// <param name="vihollinen"></param>
    /// <returns> </returns>
    private void VihollinenTippuu(PhysicsObject vihollinen)
    {
        // PhysicsObject p = new PhysicsObject(10, 10, Shape.Circle);
        vihollinen.Position = RandomGen.NextVector(Level.BoundingRect);
        // p.Color = RandomGen.NextColor();
    }

    /// <summary> 
    /// Aliohjelma joka luo peliin kentän
    /// </summary>
    /// <param name=" "></param>
    /// <returns> </returns>
    private char[,] Kentta1 =
   {
        {' ', ' ', 'M', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' '},
        {' ', ' ', '=', ' ', ' ', '=', ' ', ' ', ' ', ' ', ' '},
        {' ', ' ', ' ', '=', ' ', ' ', '=', ' ', ' ', ' ', ' '},
        {'*', ' ', ' ', ' ', ' ', '=', ' ', ' ', '=', '=', '*'},
        {'=', ' ', ' ', ' ', '=', ' ', ' ', '=', ' ', ' ', '='},
        {' ', '=', ' ', ' ', ' ', '=', ' ', ' ', '*', ' ', ' '},
        {' ', ' ', '=', ' ', ' ', ' ', '=', ' ', '=', ' ', ' '},
        {' ', ' ', ' ', '=', ' ', '=', ' ', ' ', ' ', '=', 'N'},
    };

    /// <summary> 
    /// Aliohjelma joka luo peliin kentän
    /// </summary>
    /// <param name=" "></param>
    /// <returns> </returns>
    private char[,] Kentta2 =
    {

        {' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', 'M', ' '},
        {' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', '=', ' '},
        {' ', ' ', ' ', ' ', ' ', ' ', ' ', '=', ' ', ' ', ' '},
        {' ', '*', '=', ' ', ' ', '=', ' ', ' ', ' ', ' ', '*'},
        {' ', '=', ' ', '=', '=', ' ', ' ', ' ', '=', '=', '='},
        {' ', ' ', ' ', ' ', ' ', '=', ' ', '=', ' ', ' ', ' '},
        {' ', 'N', ' ', ' ', ' ', ' ', ' ', '*', '=', '*', '*'},
        {'=', '=', ' ', '=', ' ', ' ', '=', '=', '=', '=', ' '},
    };


}