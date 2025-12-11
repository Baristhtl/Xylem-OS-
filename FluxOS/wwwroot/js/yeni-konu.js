function konuKaydet(event) {
    event.preventDefault();

    const baslik = document.getElementById('topic-title').value;
    const kategori = document.getElementById('topic-category').value;
    const icerik = document.getElementById('topic-content').value;

    if(baslik.trim() === "" || icerik.trim() === "") {
        alert("Lütfen tüm alanları doldurun!");
        return false;
    }

    // 1. Yeni Konu Objesi Oluştur
    // Her konuya benzersiz bir ID veriyoruz (Date.now() milisaniye cinsinden zamanı verir)
    const yeniKonu = {
        id: Date.now(), 
        baslik: baslik,
        kategori: kategori,
        icerik: icerik,
        yazar: "Ziyaretçi", // Şimdilik sabit, ilerde giriş yapan kişi olacak
        tarih: "Az önce",
        goruntulenme: 0,
        yanit: 0
    };

    // 2. Mevcut Konuları Hafızadan Çek (Yoksa boş liste başlat)
    let konular = JSON.parse(localStorage.getItem('forum_konulari')) || [];

    // 3. Yeni konuyu listenin EN BAŞINA ekle
    konular.unshift(yeniKonu);

    // 4. Güncel listeyi tekrar hafızaya kaydet
    localStorage.setItem('forum_konulari', JSON.stringify(konular));

    // --- ÖNEMLİ: Detay Sayfası İçin Veriyi de Hazırla ---
    // Daha önce yaptığımız "sanal veritabanı" mantığına uygun olsun diye:
    // Bu kısmı ilerde veritabanı yapınca sileceğiz, şimdilik lazım.
    let detayVerileri = JSON.parse(localStorage.getItem('detay_icerikleri')) || {};
    detayVerileri[yeniKonu.id] = {
        baslik: yeniKonu.baslik,
        etiket: "YENİ",
        etiketSinifi: "tag-question",
        yazar: yeniKonu.yazar,
        avatarHarf: "Z",
        avatarRenk: "#6c63ff",
        icerik: yeniKonu.icerik.replace(/\n/g, "<br>") // Satır başlarını <br> yap
    };
    localStorage.setItem('detay_icerikleri', JSON.stringify(detayVerileri));


    alert("Konunuz yayınlandı! Yönlendiriliyorsunuz...");
    window.location.href = "form.html";
}