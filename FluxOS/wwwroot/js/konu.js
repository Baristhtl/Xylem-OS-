// --- SANAL VERİ TABANI (Konu İçerikleri Buradan Gelecek) ---
const konuVerileri = {
    "101": {
        baslik: "Nvidia 550 sürücüsü kurulurken siyah ekran",
        etiket: "SORU",
        etiketSinifi: "tag-question", // CSS'deki renk sınıfı
        yazar: "kemal_b",
        avatarHarf: "K",
        avatarRenk: "#e11d48",
        icerik: `Merhaba arkadaşlar, TechVision OS'u yeni kurdum. Her şey harika ama Nvidia sürücülerini kurarken şu komutu girdikten sonra siyah ekranda kalıyorum:
                 <br><br><pre><code>sudo pacman -S nvidia-dkms nvidia-utils</code></pre><br>
                 Bilgisayarı yeniden başlatınca tty ekranına bile düşmüyor. Yardımcı olur musunuz?`
    },
    "102": {
        baslik: 'Pacman güncellemesinde "PGP Key" hatası alıyorum',
        etiket: "HATA",
        etiketSinifi: "tag-bug", // Sarı renk için (CSS'de varsa) yoksa tag-question kalabilir
        yazar: "user123",
        avatarHarf: "U",
        avatarRenk: "#fcd535",
        icerik: `Sistemi güncellerken sürekli PGP Key hatası alıyorum. Keyring'i güncellemeyi denedim ama işe yaramadı. Hata çıktısı şöyle:
                 <br><br><pre><code>error: failed to commit transaction (invalid or corrupted package)</code></pre><br>
                 Ne yapmam gerekiyor?`
    },
    "103": {
        baslik: "Sizce en iyi terminal emülatörü hangisi? (Kitty vs Alacritty)",
        etiket: "SOHBET",
        etiketSinifi: "tag-sticky", // Mor renk
        yazar: "mert_can",
        avatarHarf: "M",
        avatarRenk: "#6c63ff",
        icerik: `Arkadaşlar terminal değiştirmek istiyorum. GPU hızlandırmalı bir şey arıyorum. Kitty mi daha iyi yoksa Alacritty mi? Deneyimlerinizi yazar mısınız?`
    }
};

// --- URL'DEN ID ÇEKME ---
function getTopicID() {
    const urlParams = new URLSearchParams(window.location.search);
    const id = urlParams.get('id');
    return id ? id : '101'; // ID yoksa varsayılan 101 açılır
}

const KONU_ID = getTopicID();
let currentCount = 0;

// --- SAYFA YÜKLENİNCE ÇALIŞACAKLAR ---
document.addEventListener("DOMContentLoaded", function() {
    
    // A) SAYFA İÇERİĞİNİ DOLDUR (ŞABLONU DOLDURMA)
    // Önce elle yazdıklarımıza bak (Statik veriler)
    let veri = konuVerileri[KONU_ID];

    // Eğer statiklerde yoksa, LocalStorage'daki "detay_icerikleri"ne bak
    if (!veri) {
        let dinamikVeriler = JSON.parse(localStorage.getItem('detay_icerikleri')) || {};
        veri = dinamikVeriler[KONU_ID];
    }

    if (veri) {
        // Başlık ve Etiket
        document.getElementById('konu-basligi').innerText = veri.baslik;
        const etiketSpan = document.getElementById('konu-etiketi');
        etiketSpan.innerText = veri.etiket;
        etiketSpan.className = "tag " + veri.etiketSinifi; // Renk sınıfını ata

        // Yazar Bilgileri
        document.getElementById('konu-yazari').innerText = veri.yazar;
        document.getElementById('yazar-adi').innerText = veri.yazar;
        
        const avatarKutu = document.getElementById('yazar-avatar');
        avatarKutu.innerText = veri.avatarHarf;
        avatarKutu.style.backgroundColor = veri.avatarRenk;

        // Ana İçerik (innerHTML kullanıyoruz çünkü içinde <br> ve <pre> etiketleri var)
        document.getElementById('konu-icerigi').innerHTML = veri.icerik;
    } else {
        document.getElementById('konu-basligi').innerText = "Konu Bulunamadı!";
        document.getElementById('konu-icerigi').innerText = "Aradığınız konu silinmiş veya mevcut değil.";
    }


    // B) YANIT SAYISINI HAFIZADAN ÇEK
    let kayitliYanitSayisi = localStorage.getItem('reply_' + KONU_ID);
    const countSpan = document.getElementById('reply-count');

    if (kayitliYanitSayisi) {
        currentCount = parseInt(kayitliYanitSayisi);
    } else {
        // Eğer bu konuya hiç yorum yapılmamışsa HTML'deki varsayılan sayı kalmasın, 0 olsun.
        // Ancak demo olduğu için Nvidia'da (101) 12 yanıt var gibi davranabiliriz.
        if (KONU_ID === '101') currentCount = 12;
        else if (KONU_ID === '102') currentCount = 3;
        else currentCount = 0;
    }
    if(countSpan) countSpan.innerText = currentCount;
});

// --- YANIT EKLEME ---
function addReply() {
    const textArea = document.getElementById('reply-text');
    const message = textArea.value;
    const container = document.getElementById('messages-container');
    const noReplyMsg = document.getElementById('no-reply-msg'); 

    if (message.trim() === "") {
        alert("Lütfen boş mesaj girmeyin.");
        return;
    }

    if (noReplyMsg) {
        noReplyMsg.style.display = "none";
    }

    const newPostHTML = `
        <div class="post" style="border-color: #6c63ff; animation: fadeIn 0.5s;">
            <div class="user-sidebar">
                <div class="avatar" style="background: #00d26a;">S</div>
                <span class="username">Sen</span>
                <span class="user-role">Ziyaretçi</span>
            </div>
            <div class="post-content">
                <span class="post-date">Az önce</span>
                <div class="post-text">${message}</div>
            </div>
        </div>
    `;

    container.insertAdjacentHTML('beforeend', newPostHTML);
    
    currentCount = currentCount + 1;
    localStorage.setItem('reply_' + KONU_ID, currentCount);
    
    const countSpan = document.getElementById('reply-count');
    if(countSpan) {
        countSpan.innerText = currentCount;
    }

    textArea.value = "";
    window.scrollTo({ top: document.body.scrollHeight, behavior: 'smooth' });
}