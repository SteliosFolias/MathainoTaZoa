using Android.Text;
using MathainoTaZoa.Models;
using Plugin.Maui.Audio;
using System;

namespace MathainoTaZoa {
    public partial class MainPage : ContentPage {
        private List<Animal> animals;
        private int currentIndex = 0;
        private readonly IAudioManager audioManager;
        private IAudioPlayer? currentAudio;
        public MainPage(IAudioManager audioManager) {
            InitializeComponent();
            PrevBtn.IsEnabled = false;
            this.audioManager = audioManager;

            animals = new List<Animal>
 {
    new Animal { Name="Αρκούδα", Image="bear.png", NameAudio="bear_name.mp3", SoundAudio="bear.mp3" },
    new Animal { Name="Γάτα", Image="cat.png", NameAudio="cat_name.mp3", SoundAudio="cat.mp3" },
    new Animal { Name="Χιμπαντζής", Image="chimp.png", NameAudio="chimp_name.mp3", SoundAudio="chimp.mp3" },
    new Animal { Name="Γρύλος", Image="cricket.png", NameAudio="cricket_name.mp3", SoundAudio="cricket.mp3" },
    new Animal { Name="Σκύλος", Image="dog.png", NameAudio="dog_name.mp3", SoundAudio="dog.mp3" },
    new Animal { Name="Δελφίνι", Image="dolphin.png", NameAudio="dolphin_name.mp3", SoundAudio="dolphin.mp3" },
    new Animal { Name="Αετός", Image="eagle.png", NameAudio="eagle_name.mp3", SoundAudio="eagle.mp3" },
    new Animal { Name="Ελέφαντας", Image="elephant.png", NameAudio="elephant_name.mp3", SoundAudio="elephant.mp3" },
    new Animal { Name="Βάτραχος", Image="frog.png", NameAudio="frog_name.mp3", SoundAudio="frog.mp3" },
    new Animal { Name="Ιπποπόταμος", Image="hippo.png", NameAudio="hippo_name.mp3", SoundAudio="hippo.mp3" },
    new Animal { Name="Άλογο", Image="horse.png", NameAudio="horse_name.mp3", SoundAudio="horse.mp3" },
    new Animal { Name="Ιαγουάρος", Image="jaguar.png", NameAudio="jaguar_name.mp3", SoundAudio="jaguar.mp3" },
    new Animal { Name="Αρνί", Image="lamb.png", NameAudio="lamb_name.mp3", SoundAudio="lamb.mp3" },
    new Animal { Name="Λεοπάρδαλη", Image="leopard.png", NameAudio="leopard_name.mp3", SoundAudio="leopard.mp3" },
    new Animal { Name="Γουρούνι", Image="pig.png", NameAudio="pig_name.mp3", SoundAudio="pig.mp3" },
    new Animal { Name="Τίγρης", Image="tiger.png", NameAudio="tiger_name.mp3", SoundAudio="tiger.mp3" },
    new Animal { Name="Φάλαινα", Image="whale.png", NameAudio="whale_name.mp3", SoundAudio="whale.mp3" },
    new Animal { Name="Λύκος", Image="wolf.png", NameAudio="wolf_name.mp3", SoundAudio="wolf.mp3" },
    new Animal { Name="Λιοντάρι", Image="lion.png", NameAudio="lion_name.mp3", SoundAudio="lion.mp3" },
    new Animal { Name="Κότα", Image="chicken.png", NameAudio="chicken_name.mp3", SoundAudio="chicken.mp3" },
    new Animal { Name="Κόκορας", Image="rooster.png", NameAudio="rooster_name.mp3", SoundAudio="rooster.mp3" },
    new Animal { Name="Ταύρος", Image="bull.png", NameAudio="bull_name.mp3", SoundAudio="bull.mp3" },
    new Animal { Name="Πάπια", Image="duck.png", NameAudio="duck_name.mp3", SoundAudio="duck.mp3" },
    new Animal { Name="Αγελάδα", Image="cow.png", NameAudio="cow_name.mp3", SoundAudio="cow.mp3" },
  };

            // Shuffle λίστα για τυχαία σειρά
            var rnd = new Random();
            animals = animals.OrderBy(x => rnd.Next()).ToList();
            ShowAnimal();
        }
        private void StopCurrentAudio() {
            if (currentAudio != null) {
                currentAudio.Stop();
                currentAudio.Dispose();
                currentAudio = null;
            }
        }
        private void ShowAnimal() {
            var animal = animals[currentIndex];
            AnimalImage.Source = animal.Image;
            AnimalLabel.Text = animal.Name;
            AnimalImage.HorizontalOptions = LayoutOptions.Center;
            AnimalImage.VerticalOptions = LayoutOptions.Center;
        }
        private async void OnAnimalTapped(object sender, EventArgs e) {
            var animal = animals[currentIndex];
            AnimalImage.IsEnabled = false;
            // Σταματάει προηγούμενο ήχο αν παίζει
            StopCurrentAudio();

            // Παίζει πρώτα το όνομα
            try {
                currentAudio = audioManager.CreatePlayer(await FileSystem.OpenAppPackageFileAsync(animal.NameAudio));
                currentAudio.Play();
                await Task.Delay(TimeSpan.FromSeconds(currentAudio.Duration + 1));
                currentAudio.Stop();
                currentAudio.Dispose();
                currentAudio = null;
            }
            catch { }
            // Εμφανίζει το GIF
            var gifFile = await FileSystem.OpenAppPackageFileAsync($"{animal.Image.Split(".png").FirstOrDefault()}_move.gif");
            using var ms = new MemoryStream();
            await gifFile.CopyToAsync(ms);
            var base64 = Convert.ToBase64String(ms.ToArray());
            AnimalGifWeb.Source = new HtmlWebViewSource {
                Html = $"<html><body style='margin:0;padding:0;background:#fff;'><img src='data:image/gif;base64,{base64}' width='100%' height='100%'/></body></html>"
            };

            AnimalGifWeb.IsVisible = true;
            AnimalImage.IsVisible = false;

            // Παίζει τον ήχο του ζώου
            try {
                currentAudio = audioManager.CreatePlayer(await FileSystem.OpenAppPackageFileAsync(animal.SoundAudio));
                currentAudio.Play();
                await Task.Delay(TimeSpan.FromSeconds(currentAudio.Duration));
                currentAudio.Stop();
                currentAudio.Dispose();
                currentAudio = null;
            }
            catch { }
            // Επαναφορά εικόνας
            AnimalGifWeb.IsVisible = false;
            AnimalImage.IsVisible = true;
            AnimalImage.IsEnabled = true; 
        }
        private void OnNextClicked(object sender, EventArgs e) {
            // Επαναφορά εικόνας
            AnimalGifWeb.IsVisible = false;
            AnimalImage.IsVisible = true;
            AnimalGifWeb.Source = null; 
            PrevBtn.IsEnabled = true;
            NextBtn.IsEnabled = true;
            StopCurrentAudio(); // Σταματάει τον ήχο πριν αλλάξει ζώο
            currentIndex = (currentIndex + 1) % animals.Count;
            if (currentIndex == animals.Count - 1) NextBtn.IsEnabled = false;
            ShowAnimal();
        }
        private void OnPrevClicked(object sender, EventArgs e) {
            // Επαναφορά εικόνας
            AnimalGifWeb.IsVisible = false;
            AnimalImage.IsVisible = true;
            AnimalGifWeb.Source = null;
            PrevBtn.IsEnabled = true;
            NextBtn.IsEnabled = true;
            StopCurrentAudio(); // Σταματάει τον ήχο πριν αλλάξει ζώο
            currentIndex = (currentIndex - 1 + animals.Count) % animals.Count;
            if (currentIndex == 0) PrevBtn.IsEnabled = false;
            ShowAnimal();
        }
    }
}