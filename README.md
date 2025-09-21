# 📚 SwipeLearn – AI Destekli Eğitim Platformu

**SwipeLearn**, kullanıcıların seçtiği ders konularını yapay zekâ destekli içeriklere dönüştüren bir eğitim platformudur.
Sistem; **ChatGPT** ile konu anlatımları üretir, **Fal.AI** ile anlatımı görsellerle destekler, **ElevenLabs** ile metinleri seslendirir ve **FFmpeg** yardımıyla bu içerikleri bir araya getirerek eğitici videolar oluşturur. Videoların sonunda ise öğrencilere, öğrendiklerini pekiştirmeleri için quiz sunulur.

Bu sayede öğrenciler; **dinleyerek, görerek ve sorularla kendini test ederek** çok yönlü bir öğrenme deneyimi yaşar. Amaç, konuların daha kalıcı, etkili ve eğlenceli şekilde öğrenilmesini sağlamaktır.

---


## 🎥 Demo
[SwipeLearn](https://swipelearn.serkanbayram.dev/)  =>  (https://swipelearn.serkanbayram.dev/)

---
## 🚀 Özellikler

-  **Konu Açıklamaları** → Kullanıcı ders konusunu girer, ChatGPT konuyu açıklayan bir metin oluşturur.  
-  **Görsel Üretimi** → Açıklamaya uygun görseller Fal.AI (`flux-1/schnell`) modeli ile otomatik üretilir.  
-  **Seslendirme** → ElevenLabs API ile metin seslendirilir.  
-  **Video Üretimi** → Görseller ve ses FFmpeg ile birleştirilerek eğitici bir video hazırlanır.  
-  **Quiz Modülü** → Video sonunda 10 soruluk quiz ile konunun pekiştirilmesi sağlanır.  
-  **Yanlış Cevap Açıklamaları** → Öğrenciler yanlış cevapladığında, soruya özel ChatGPT açıklaması ile doğru bilgi aktarılır.  

---

## 🛠 Kullanılan Teknolojiler

**Backend**
- [.NET 8 Web API](https://dotnet.microsoft.com/)  
- [Swagger](https://swagger.io/) – API dokümantasyonu  
- [PostgreSQL](https://www.postgresql.org/) – Veritabanı  
- Entity Framework Core  

**Frontend**
- [React](https://react.dev/)  
- [Vite](https://vitejs.dev/) – Hızlı geliştirme ortamı  
- [shadcn/ui](https://ui.shadcn.com/) – Modern UI bileşenleri  
- [TanStack Query](https://tanstack.com/query) – Veri fetch & cache yönetimi  

**3rd Party API & Tools**
- [OpenAI / ChatGPT](https://platform.openai.com/) – Konu açıklamaları  
- [Fal.AI](https://fal.ai/) – Görsel üretimi (flux-1/schnell modeli)  
- [ElevenLabs](https://elevenlabs.io/) – Seslendirme  
- [FFmpeg](https://ffmpeg.org/) – Video işleme  

---

## ⚙️ Kurulum

### 1. Gereksinimler
- [.NET 8 SDK](https://dotnet.microsoft.com/download)  
- [Node.js (>=18.x)](https://nodejs.org/)  
- [PostgreSQL](https://www.postgresql.org/download/)  
- [FFmpeg](https://ffmpeg.org/download.html)  

### 2. Ortam Değişkenleri
Backend klasörüne `.development.env` dosyası ekleyin:

```env
DATABASE_URL=Host=host_name;Port=port_number;Database=database_name;Username=username;Password=password
CHATGPT_API_KEY=your_openai_api_key
FALAI_API_KEY=your_falai_api_key
ELEVENLABS_API_KEY=your_elevenlabs_api_key
```

### 3. Veritabanı Kurulumu

```bash
# PostgreSQL'e yedeği yükle
psql -U your_user -d swipelearn -f Database/swipeLearnBackup.sql
```

### 4. Backend Çalıştırma

```bash
cd SwipeLearn
dotnet restore
dotnet run
```

API Swagger dokümantasyonu şu adreste açılır:
👉 [http://localhost:5000/swagger](http://localhost:5000/swagger)

### 5. Frontend Çalıştırma

```bash
cd SwipeLearnFrontend
npm install
npm run dev
```

Uygulama şu adreste açılır:
👉 [http://localhost:5173](http://localhost:5173)

---

## 📌 Örnek Kullanım

Kullanıcı konu olarak:

```
İstanbul'un fethi
```

girdiğinde sistem:

1. ChatGPT’den açıklama alır.
2. Fal.AI ile görseller üretir.
3. ElevenLabs ile seslendirir.
4. FFmpeg ile videoyu birleştirir.
5. Video sonunda quiz sunar.

---

## 🧪 Mantıklı Çalışan Prompt Örnekleri

* `1. Dünya Savaşı`
* `99 Depremi`
* `İstanbul'un fethi`

