using Microsoft.AspNetCore.Components.Authorization;
using StajMulakatuygulaması.Models; // Kullanıcı modelinizin olduğu yer
using System.Security.Claims;

namespace StajMulakatuygulamasi.Services
{
    public class CustomAuthenticationStateProvider : AuthenticationStateProvider
    {
        // Varsayılan olarak kimse giriş yapmamış (Anonymous) kabul ediyoruz
        private ClaimsPrincipal _currentUser = new ClaimsPrincipal(new ClaimsIdentity());

        public override Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            return Task.FromResult(new AuthenticationState(_currentUser));
        }

        // Giriş yapıldığında bu metodu çağıracağız
        public void MarkUserAsAuthenticated(Kullanicilar user)
        {
            // Kullanıcının bilgilerini (Claim) oluşturuyoruz
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Role, user.Role.RoleName),
                // ID'yi string olarak saklıyoruz, lazım olursa int'e çeviririz
                new Claim("UserId", user.UserId.ToString())
            };

            var identity = new ClaimsIdentity(claims, "CustomAuth");
            _currentUser = new ClaimsPrincipal(identity);

            // Sisteme "Durum değişti, arayüzü güncelle!" diyoruz
            NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(_currentUser)));
        }

        // Çıkış yapıldığında bu metodu çağıracağız
        public void MarkUserAsLoggedOut()
        {
            _currentUser = new ClaimsPrincipal(new ClaimsIdentity());
            NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(_currentUser)));
        }
    }
}