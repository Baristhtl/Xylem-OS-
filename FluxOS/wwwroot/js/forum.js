document.addEventListener("DOMContentLoaded", function() {

    // --- A) YENİ EKLENEN KONULARI LİSTEYE DÖK ---
    const topicContainer = document.getElementById('topic-container');
    
    // Hafızadan konuları çek
    let konular = JSON.parse(localStorage.getItem('forum_konulari')) || [];

    // Her bir konu için HTML oluştur ve sayfaya ekle
    konular.forEach(function(konu) {
        
        let htmlKart = `
        <div class="topic-card" style="border-left: 4px solid #6c63ff; animation: fadeIn 0.5s;">
            <div class="topic-header">
                <span class="tag-question" style="background: #6c63ff; color:white; padding: 2px 8px; border-radius: 4px; font-size: 0.8rem;">YENİ</span>
                
                <a href="konu.html?id=${konu.id}" onclick="goruntulenmeArtir(${konu.id})" class="topic-link" style="color: white; text-decoration: none; font-weight: bold; margin-left: 10px;">
                     ${konu.baslik}
                </a>
            </div>

            <div class="topic-info" style="color: #94a3b8; font-size: 0.9rem; margin: 10px 0;">
                ${konu.yazar} tarafından • ${konu.kategori} • ${konu.tarih}
            </div>

            <div class="topic-stats" style="display: flex; gap: 15px;">
                <div class="stat">
                    <span class="yanit-sayac" data-id="${konu.id}" style="color: white; font-weight: bold;">${konu.yanit}</span>
                    <span style="color: #64748b;">Yanıt</span>
                </div>

                <div class="stat">
                    <span class="sayac" data-id="${konu.id}" style="color: white; font-weight: bold;">${konu.goruntulenme}</span>
                    <span style="color: #64748b;">Gör.</span>
                </div>
            </div>
        </div>
        `;

        // Yeni kartı listenin en başına (ilk elemanından önce) ekle
        topicContainer.insertAdjacentHTML('afterbegin', htmlKart);
    });


    // --- B)   SAYAÇ KODLARI ---
    
    // 1. GÖRÜNTÜLENME SAYILARINI YÜKLE...
    let tumGoruntulenmeler = document.querySelectorAll('.sayac');
    // ... (Kodun geri kalanı aynı) ...
});

// Tıklama Fonksiyonu (Genel)
function goruntulenmeArtir(id) {
    // 1. Anahtar ismini belirle (Örn: view_101)
    let hafizaAnahtari = 'view_' + id;

    // 2. Mevcut sayıyı hafızadan al
    let mevcutSayi = localStorage.getItem(hafizaAnahtari);

    if (!mevcutSayi) {
        // O anki HTML elementini bulup içindeki sayıyı başlangıç kabul edebiliriz
        let ilgiliKutu = document.querySelector(`.sayac[data-id="${id}"]`);
        mevcutSayi = ilgiliKutu ? parseInt(ilgiliKutu.innerText) : 0;
    }

    // 3. Sayıyı 1 artır
    let yeniSayi = parseInt(mevcutSayi) + 1;

    // 4. Hafızaya yeni sayıyı kaydet
    localStorage.setItem(hafizaAnahtari, yeniSayi);

    // 5. Ekranda anlık olarak güncelle
    let guncellenecekKutu = document.querySelector(`.sayac[data-id="${id}"]`);
    if (guncellenecekKutu) {
        guncellenecekKutu.innerText = yeniSayi;
    }
}
