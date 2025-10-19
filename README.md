<div style="text-align: center;">
    <img alt="MemQuran Logo" src="https://assets.zyrosite.com/cdn-cgi/image/format=auto,h=128,fit=crop/AzG7nVE3kWTvz3Wn/mq1-Ylevj927ykS7xa11.png">
</div>

# MemQuran API [![Build & Test](https://github.com/quranicapps/memquran-api/actions/workflows/ci_main.yml/badge.svg)](https://github.com/quranicapps/memquran-api/actions/workflows/ci_main.yml)

MemQuran API is a robust service for accessing Quranic resources such as Surahs, Juzs, Audio, Duas, Reciters etc. It is designed to be for local testing that easily integrates into web, mobile, and desktop applications.

### Use without running the API, just use the CDN:

If you just want the data and files, you can use the jsDelivr CDN link e.g. (https://cdn.jsdelivr.net/gh/quranstatic/static@v0.0.63/json/surahs/surah_1.json) directly in your applications. This data is FREE and already cached globally for you.

This api is a wrapper around the static JSON files and images provided by the [QuranStatic JSON source files Repository on GitHub](https://github.com/quranstatic/static).
QuranStaic is a project that provides static JSON files and images for the Quran, including translations, tafsirs, and recitations. The files are organized in a structured manner, making it easy to access specific data. These files are then cached on [jsDelivr](https://www.jsdelivr.com/) CDN for
fast and reliable access.

[jsDelivr](https://www.jsdelivr.com/) is a free, global Content Delivery Network (CDN) for open-source files, primarily serving content from npm and GitHub. It provides developers with reliable and fast delivery of JavaScript, CSS, and other web resources, permanently caching files for continuous
availability even if the source repository is changed or deleted.

You can run this locally for development or even extend it yourself to add more features, cater for your json files and run/deploy to fit your needs.

---

## Used By Applications

#### Official MemQuran app:

- <a target="_blank" href="https://apps.apple.com/us/app/quran-by-memquran-com/id6569242722"><img src="https://upload.wikimedia.org/wikipedia/commons/6/67/App_Store_%28iOS%29.svg" alt="App Store" width="24" style="vertical-align:middle;"/> MemQuran in the iOS App Store</a>

- <a target="_blank" href="https://play.google.com/store/apps/details?id=com.quranicapps.memquran"><img src="https://upload.wikimedia.org/wikipedia/commons/7/78/Google_Play_Store_badge_EN.svg" alt="Google Play" height="24" style="vertical-align:middle;"/> MemQuran in the Google Play Store</a>

#### Official MemQuran website:

- <a href="https://www.memquran.com/" target="_blank">MemQuran Website</a>

---

## Table of Contents

- [Features](#features)
- [Tech Stack](#tech-stack)
- [Installation & Setup](#installation--setup)
- [Configuration](#configuration)
- [Authentication](#authentication)
- [Endpoints](#endpoints)
- [Error Handling](#error-handling)
- [Usage Examples](#usage-examples)
- [Observability & Diagnostics](#observability--diagnostics)
- [Project Layout](#project-layout)
- [Troubleshooting](#troubleshooting)
- [Contributing](#contributing)
- [License](#license)

---

## Features

- Retrieve surahs and verses with translations and tafsirs
- Health checks and diagnostics
- Interactive OpenAPI/Swagger documentation (https://localhost:3123/swagger/index.html)
- Observability (OpenTelemetry), caching (Redis), and logging (Seq)
- Access Quranic resources: surahs, rukus, tafsirs, editions, duas, images, search

---

## Tech Stack

- **.NET 9, ASP.NET Core**
- **Swagger/OpenAPI** for API docs
- **Redis** for hybrid/distributed caching
- **HealthChecks** and **HealthChecks UI**
- **OpenTelemetry** (OTLP and console exporters)
- **Seq** for structured logging
- **Messaging** for receiving events (AWS, RabbitMQ, Azure Service Bus, etc.) using [Topica, a lightweight, modular library for managing messages and topics in .NET](https://github.com/rehmanab/topica)

---

## Installation & Setup

### Prerequisites

- [.NET SDK 9.0+](https://dotnet.microsoft.com/download)
- Optional: Redis, Docker Desktop, Seq, OTLP collector

### Running Locally

```bash
cd \src\MemQuran.Api
dotnet run
```

---

## Configuration

- Local settings: `appsettings.json`, `appsettings.Development.json`
- Secrets: `dotnet user-secrets` (see `UserSecretsId` in `.csproj`)
- Common settings:
  - Content Delivery Network, Cache settings
  - Redis connection string
  - Seq server URL and API key
  - OTLP endpoint

---

## Endpoints

Please find a Postman collection in the root directory: `.postman/MemQuran-API.postman_collection.json`, this collection has all the endpoints, after importing into Postman please add a new environment with the following variables:

- `BaseUrl`: The base URL of the API.

1. `https://localhost:3123` Localhost
2. `https://cdn.jsdelivr.net/gh/quranstatic/static@v0.0.63` jsDelivr CDN (Use this for public access without hosting the API yourself)

> Remember all the json files are in the [QuranStatic JSON source files Repository on GitHub](https://github.com/quranstatic/static), so you can construct the CDN url easily by using `https://cdn.jsdelivr.net/gh/quranstatic/static@v0.0.63` plus the file path you need constructed from the root of the repository, please see examples below.

### Audio

> Get Surah audio timings for a reciter.
- **GET `/json/audio/{reciter}/timings/surah/{timings_file}`**


  - `https://localhost:3123/json/audio/abderahmane-eloosi-1/timings/surah/abderahmane-eloosi-1_timings_surah_1.json`
  - `https://cdn.jsdelivr.net/gh/quranstatic/static@v0.0.63/json/audio/abderahmane-eloosi-1/timings/surah/abderahmane-eloosi-1_timings_surah_1.json`

  ```json
  {
    "Type": "surah",
    "Number": 1,
    "DefaultUrl": "001.mp3",
    "TotalDurationMs": 74702,
    "Segments": [
        {
            "I": 0,
            "S": 1,
            "V": 1,
            "Url": "001.mp3",
            "StartMs": 27800,
            "EndMs": 34657,
            "DurationMs": 6857
        },
        ...
  ```

  > Get Juz audio timings for a reciter.
- **GET `/json/audio/{reciter}/timings/juz/{timings_file}`**


  - `https://localhost:3123/json/audio/abdulbaset-mujawwad-2/timings/juz/abdulbaset-mujawwad-2_timings_juz_1.json`
  - `https://cdn.jsdelivr.net/gh/quranstatic/static@v0.0.63/json/audio/abdulbaset-mujawwad-2/timings/juz/abdulbaset-mujawwad-2_timings_juz_1.json`

  ```json
  {
    "Type": "juz",
    "Number": 1,
    "DefaultUrl": "https://download.quranicaudio.com/quran/abdulbaset_mujawwad/001.mp3",
    "TotalDurationMs": null,
    "Segments": [
        {
            "I": 0,
            "S": 1,
            "V": 1,
            "Url": "https://download.quranicaudio.com/quran/abdulbaset_mujawwad/001.mp3",
            "StartMs": 6064,
            "EndMs": 14816,
            "DurationMs": 8752
        },
        ...
  ```

  > Get Page audio timings for a reciter.
- **GET `/json/audio/{reciter}/timings/page/{timings_file}`**


  - `https://localhost:3123/json/audio/abdulbaset-mujawwad-2/timings/page/abdulbaset-mujawwad-2_timings_page_1.json`
  - `https://cdn.jsdelivr.net/gh/quranstatic/static@v0.0.63/json/audio/abdulbaset-mujawwad-2/timings/page/abdulbaset-mujawwad-2_timings_page_1.json`

```json
{
    "Type": "page",
    "Number": 1,
    "DefaultUrl": "https://download.quranicaudio.com/quran/abdulbaset_mujawwad/001.mp3",
    "TotalDurationMs": null,
    "Segments": [
        {
            "I": 0,
            "S": 1,
            "V": 1,
            "Url": "https://download.quranicaudio.com/quran/abdulbaset_mujawwad/001.mp3",
            "StartMs": 6064,
            "EndMs": 14816,
            "DurationMs": 8752
        },
        ...
```

> Get Ruku audio timings for a reciter.
- **GET `/json/audio/{reciter}/timings/ruku/{timings_file}`**


  - `https://localhost:3123/json/audio/abdulbaset-mujawwad-2/timings/ruku/abdulbaset-mujawwad-2_timings_ruku_1.json`
  - `https://cdn.jsdelivr.net/gh/quranstatic/static@v0.0.63/json/audio/abdulbaset-mujawwad-2/timings/ruku/abdulbaset-mujawwad-2_timings_ruku_1.json`

```json
{
    "Type": "ruku",
    "Number": 1,
    "DefaultUrl": "https://download.quranicaudio.com/quran/abdulbaset_mujawwad/001.mp3",
    "TotalDurationMs": null,
    "Segments": [
        {
            "I": 0,
            "S": 1,
            "V": 1,
            "Url": "https://download.quranicaudio.com/quran/abdulbaset_mujawwad/001.mp3",
            "StartMs": 6064,
            "EndMs": 14816,
            "DurationMs": 8752
        },
        ...
```

> Get Maqra audio timings for a reciter.
- **GET `/json/audio/{reciter}/timings/maqra/{timings_file}`**


  - `https://localhost:3123/json/audio/abdulbaset-mujawwad-2/timings/maqra/abdulbaset-mujawwad-2_timings_maqra_1.json`
  - `https://cdn.jsdelivr.net/gh/quranstatic/static@v0.0.63/json/audio/abdulbaset-mujawwad-2/timings/maqra/abdulbaset-mujawwad-2_timings_maqra_1.json`

```json
{
    "Type": "maqra",
    "Number": 1,
    "DefaultUrl": "https://download.quranicaudio.com/quran/abdulbaset_mujawwad/001.mp3",
    "TotalDurationMs": null,
    "Segments": [
        {
            "I": 0,
            "S": 1,
            "V": 1,
            "Url": "https://download.quranicaudio.com/quran/abdulbaset_mujawwad/001.mp3",
            "StartMs": 6064,
            "EndMs": 14816,
            "DurationMs": 8752
        },
        ...
```

> Get Dua audio file.
- **GET `/audio/duas/{dua_file}`**


  - `https://localhost:3123/audio/duas/1_1.mp3`
  - `https://cdn.jsdelivr.net/gh/quranstatic/static@v0.0.63/audio/duas/1_1.mp3`

```json
Audio File
```

> Get Tajweed audio file.
- **GET `/audio/tajweed/{tajweed_file}`**


  - `https://localhost:3123/audio/tajweed/ikhfa.mp3`
  - `https://cdn.jsdelivr.net/gh/quranstatic/static@v0.0.63/audio/tajweed/ikhfa.mp3`

```json
Audio File
```

> Get Word-by-Word audio file.
- **GET `/audio/wbw/{wbw_file}`**


  - `https://localhost:3123/audio/wbw/001_001_001.mp3`
  - `https://cdn.jsdelivr.net/gh/quranstatic/static@v0.0.63/audio/wbw/001_001_001.mp3`

```json
Audio File
```

> Get Memorise audio file.
- **GET `/audio/memorise/{memorise_file}`**


  - `https://localhost:3123/audio/memorise/0A5639E55EA4CF708D349C6FC8D95BE7CED289AFC0875F5F306CA3D3ECDA3CE9.mp3`
  - `https://cdn.jsdelivr.net/gh/quranstatic/static@v0.0.63/audio/memorise/0A5639E55EA4CF708D349C6FC8D95BE7CED289AFC0875F5F306CA3D3ECDA3CE9.mp3`

```json
Audio File
```

> Get Correct answer sound.
- **GET `/audio/common/correct.mp3`**


  - `https://localhost:3123/audio/common/correct.mp3`
  - `https://cdn.jsdelivr.net/gh/quranstatic/static@v0.0.63/audio/common/correct.mp3`

```json
Audio File
```

> Get Incorrect answer sound.
- **GET `/audio/common/incorrect.mp3`**


  - `https://localhost:3123/audio/common/incorrect.mp3`
  - `https://cdn.jsdelivr.net/gh/quranstatic/static@v0.0.63/audio/common/incorrect.mp3`

```json
Audio File
```

> Get Names of Allah audio file.
- **GET `/audio/namesOfAllah/{number}.mp3`**


  - `https://localhost:3123/audio/namesOfAllah/1.mp3`
  - `https://cdn.jsdelivr.net/gh/quranstatic/static@v0.0.63/audio/namesOfAllah/1.mp3`

```json
Audio File
```

### Duas

> Get Duas in English locale.
- **GET `/json/duas/en_duas.json`**


  - `https://localhost:3123/json/duas/en_duas.json`
  - `https://cdn.jsdelivr.net/gh/quranstatic/static@v0.0.63/json/duas/en_duas.json`

```json
{
    "Hadith-66": [
        {
            "Id": "Hadith-66:1:1",
            "DuaType": 2,
            "LanguageCode": "en",
            "Title": "About protection against shaitan's instigations at the time of prayer or recitation",
            "Text": "أَعُوذُ بِاللهِ مِنَ الشَّيْطّانِ الرَّجِيمِ",
            "Transliteration": "'A'oothu billaahi minash-Shaytaanir-rajeem.",
            "Translation": "I seek refuge in Allah from Satan the outcast.",
            "AudioFileName": "42_1.mp3"
        }
    ],
    ...
```

### Editions

> Get list of Tafsir editions.
- **GET `/json/editions/tafsirEditions.json`**


  - `https://localhost:3123/json/editions/tafsirEditions.json`
  - `https://cdn.jsdelivr.net/gh/quranstatic/static@v0.0.63/json/editions/tafsirEditions.json`

```json
[
    {
        "Id": 14,
        "LanguageCodes": [
            "ar",
            "ara"
        ],
        "Name": "Tafsir Ibn Kathir",
        "Author": "Hafiz Ibn Kathir",
        "Language": "arabic",
        "Direction": "rtl",
        "Source": "https://quran.com/"
    },
    ...
```

> Get list of Translation editions. 132 is English (Mustafa Khattab Allah Edition).
- **GET `/json/editions/translationEditions.json`**


  - `https://localhost:3123/json/editions/translationEditions.json`
  - `https://cdn.jsdelivr.net/gh/quranstatic/static@v0.0.63/json/editions/translationEditions.json`

```json
[
    {
        "Id": 1,
        "LanguageCodes": [
            "aa",
            "aar"
        ],
        "Name": "aar-sheikhmahmoudab",
        "Author": "Sheikh Mahmoud Abdel Qader Hamz And Group Of Scholars",
        "Language": "Afar",
        "Description": "Afar",
        "IsTransliteration": false,
        "HasDiacritics": false,
        "Direction": "ltr",
        "Source": "https://qurancomplex.gov.sa/",
        "ExampleText": "Faylaa kee Saare ginô Rabbi le"
    },
    ...
```

### Images

> Get Tajweed word image.
- **GET `/images/tajweed/{surah}/{verse}/{word}.png`**


  - `https://localhost:3123/images/tajweed/1/1/1.png`
  - `https://cdn.jsdelivr.net/gh/quranstatic/static@v0.0.63/images/tajweed/1/1/1.png`

```json
Image File
```

> Get Tajweed number image.
- **GET `/images/tajweed/{number}.png`**


  - `https://localhost:3123/images/tajweed/1.png`
  - `https://cdn.jsdelivr.net/gh/quranstatic/static@v0.0.63/images/tajweed/1.png`

```json
Image File
```

> Get Reciter image.
- **GET `/images/reciters/{reciter}.png`**


  - `https://localhost:3123/images/reciters/abdallah-al-matroud-1.png`
  - `https://cdn.jsdelivr.net/gh/quranstatic/static@v0.0.63/images/reciters/abdallah-al-matroud-1.png`

```json
Image File
```

### Surahs

> Get Surah info in English locale.
- **GET `/json/surahs/en_surahInfo.json`**


  - `https://localhost:3123/json/surahs/en_surahInfo.json`
  - `https://cdn.jsdelivr.net/gh/quranstatic/static@v0.0.63/json/surahs/en_surahInfo.json`

```json
{
    "LanguageCode": "en",
    "SurahInfos": [
        {
            "Chapter": 1,
            "Index": "001",
            "Name": "Al-Fatihah",
            "TranslatedName": "The Opening",
            "ArabicName": "سُوْرَةُ الْفَاتِحَةِ",
            "Revelation": "Mecca",
            "RevelationOrder": 5,
            "HasBismillah": false,
            "VerseCount": 7,
            "SajdaCount": 0,
            "JuzStart": 1,
            "JuzEnd": 1,
            "VerseStart": 1,
            "VerseEnd": 7,
            "ManzilStart": 1,
            "ManzilEnd": 1,
            "RukuStart": 1,
            "RukuEnd": 1,
            "MaqraStart": 1,
            "MaqraEnd": 1,
            "HizbStart": 1,
            "HizbEnd": 1,
            "PageStart": 1,
            "PageEnd": 1,
            "Text": "بِسۡمِ ٱللَّهِ ٱلرَّحۡمَٰنِ ٱلرَّحِيمِ ١ ٱلۡحَمۡدُ لِلَّهِ رَبِّ ٱلۡعَٰلَمِينَ ٢",
            "Summaries": null,
            "SurahVerseKeys": [
                "1:1",
                "1:2",
                "1:3",
                "1:4",
                "1:5",
                "1:6",
                "1:7"
            ]
        },
        ...
```

> Get Surah info with summary in English locale.
- **GET `/json/surahs/en_surahInfoWithSummary.json`**


  - `https://localhost:3123/json/surahs/en_surahInfoWithSummary.json`
  - `https://cdn.jsdelivr.net/gh/quranstatic/static@v0.0.63/json/surahs/en_surahInfoWithSummary.json`

```json
{
    "LanguageCode": "en",
    "SurahInfos": [
        {
            "Chapter": 1,
            "Index": "001",
            "Name": "Al-Fatihah",
            "TranslatedName": "The Opening",
            "ArabicName": "سُوْرَةُ الْفَاتِحَةِ",
            "Revelation": "Mecca",
            "RevelationOrder": 5,
            "HasBismillah": false,
            "VerseCount": 7,
            "SajdaCount": 0,
            "JuzStart": 1,
            "JuzEnd": 1,
            "VerseStart": 1,
            "VerseEnd": 7,
            "ManzilStart": 1,
            "ManzilEnd": 1,
            "RukuStart": 1,
            "RukuEnd": 1,
            "MaqraStart": 1,
            "MaqraEnd": 1,
            ...
```

> Get the Surah with its data, text and tajweed text.
- **GET `/json/surahs/surah_{number}.json`**


  - `https://localhost:3123/json/surahs/surah_1.json`
  - `https://cdn.jsdelivr.net/gh/quranstatic/static@v0.0.63/json/surahs/surah_1.json`

  ```json
  "SurahNumber": 1,
    "HasBismillah": false,
    "Verses": [
        {
            "SurahNumber": 1,
            "SurahIndex": null,
            "VerseNumber": 1,
            "ShowSurahName": false,
            "ShowBismillah": false,
            "Text": "بِسۡمِ ٱللَّهِ ٱلرَّحۡمَٰنِ ٱلرَّحِيمِ",
            ...
        }
    ]
  ```

  > Get Surah word-by-word translation in English.
- **GET `/json/surahs/surah_wbw_en_{number}.json`**


  - `https://localhost:3123/json/surahs/surah_wbw_en_1.json`
  - `https://cdn.jsdelivr.net/gh/quranstatic/static@v0.0.63/json/surahs/surah_wbw_en_1.json`

```json
{
    "SurahNumber": 1,
    "HasBismillah": false,
    "Verses": [
        {
            "SurahNumber": 1,
            "SurahIndex": "001",
            "VerseNumber": 1,
            "ShowSurahName": false,
            "ShowBismillah": false,
            "Words": [
                {
                    "Position": 1,
                    "AudioFileName": "001_001_001.mp3",
                    "TajweedImageFilePath": "1/1/1.png",
                    "Text": "بِسْمِ",
                    "Translation": "In (the) name",
                    "Transliteration": "bis'mi",
                    "Root": "س م و"
                    ...
```

> Get Surah translation for a specific edition.
- **GET `/json/surahTranslations/surah_translation_{number}_{edition}.json`**


  - `https://localhost:3123/json/surahTranslations/surah_translation_1_132.json`
  - `https://cdn.jsdelivr.net/gh/quranstatic/static@v0.0.63/json/surahTranslations/surah_translation_1_132.json`

```json
{
    "Id": 132,
    "LanguageCodes": [
        "en",
        "eng"
    ],
    "Name": "eng-mustafakhattaba",
    "Author": "Mustafa Khattab Allah Edition",
    "Language": "English",
    "Description": "English",
    "Direction": "ltr",
    "Source": "",
    "Translations": [
        {
            "SurahNumber": 1,
            "VerseNumber": 1,
            "Text": "In the Name of Allah—the Most Compassionate, Most Merciful"
        },
        ...
```

### Juzs

> Get Juz info in English locale.
- **GET `/json/juzs/en_juzInfo.json`**


  - `https://localhost:3123/json/juzs/en_juzInfo.json`
  - `https://cdn.jsdelivr.net/gh/quranstatic/static@v0.0.63/json/juzs/en_juzInfo.json`

```json
{
    "LanguageCode": "en",
    "JuzInfos": [
        {
            "Juz": 1,
            "ChapterStart": 1,
            "ChapterEnd": 2,
            "Text": "بِسۡمِ ٱللَّهِ ٱلرَّحۡمَٰنِ ٱلرَّحِيمِ ١ ٱلۡحَمۡدُ لِلَّهِ رَبِّ ٱلۡعَٰلَمِينَ ٢",
            "Summaries": null,
            "SurahVerseKeys": [
                "1:1",
                "1:2",
                "1:3",
                "1:4",
                "1:5",
                "1:6",
                "1:7",
                "2:1",
                "2:2",
                "2:3",
                ...
```

> Get Juz info with summary in English locale.
- **GET `/json/juzs/en_juzInfoWithSummary.json`**


  - `https://localhost:3123/json/juzs/en_juzInfoWithSummary.json`
  - `https://cdn.jsdelivr.net/gh/quranstatic/static@v0.0.63/json/juzs/en_juzInfoWithSummary.json`

```json
{
    "LanguageCode": "en",
    "JuzInfos": [
        {
            "Juz": 1,
            "ChapterStart": 1,
            "ChapterEnd": 2,
            "Text": "بِسۡمِ ٱللَّهِ ٱلرَّحۡمَٰنِ ٱلرَّحِيمِ ١ ٱلۡحَمۡدُ لِلَّهِ رَبِّ ٱلۡعَٰلَمِينَ ٢",
            "Summaries": [
                "<p><strong>1. Surat al-Fatiha</strong><br>",
                "        In the name of Allah, the Kind, the Caring.<br>",
                ...
```

> Get Juz data.
- **GET `/json/juzs/juz_{number}.json`**


  - `https://localhost:3123/json/juzs/juz_1.json`
  - `https://cdn.jsdelivr.net/gh/quranstatic/static@v0.0.63/json/juzs/juz_1.json`

```json
{
    "JuzNumber": 1,
    "Verses": [
        {
            "SurahNumber": 1,
            "SurahIndex": "001",
            "VerseNumber": 1,
            "ShowSurahName": true,
            "ShowBismillah": false,
            "Text": "بِسۡمِ ٱللَّهِ ٱلرَّحۡمَٰنِ ٱلرَّحِيمِ",
            "TajweedText": "بِسْمِ <tajweed class=ham_wasl>ٱ</tajweed>للَّهِ <tajweed class=ham_wasl>ٱ</tajweed><tajweed
            ...
```

> Get Juz word-by-word translation in English.
- **GET `/json/juzs/juz_wbw_en_{number}.json`**


  - `https://localhost:3123/json/juzs/juz_wbw_en_1.json`
  - `https://cdn.jsdelivr.net/gh/quranstatic/static@v0.0.63/json/juzs/juz_wbw_en_1.json`

```json
{
    "JuzNumber": 1,
    "Verses": [
        {
            "SurahNumber": 1,
            "SurahIndex": "001",
            "VerseNumber": 1,
            "ShowSurahName": true,
            "ShowBismillah": false,
            "Words": [
                {
                    "Position": 1,
                    "AudioFileName": "001_001_001.mp3",
                    "TajweedImageFilePath": "1/1/1.png",
                    "Text": "بِسْمِ",
                    "Translation": "In (the) name",
                    "Transliteration": "bis'mi",
                    "Root": "س م و"
                    ...
```

> Get Juz translation for a specific edition.
- **GET `/json/juzTranslations/juz_translation_{number}_{edition}.json`**


  - `https://localhost:3123/json/juzTranslations/juz_translation_1_132.json`
  - `https://cdn.jsdelivr.net/gh/quranstatic/static@v0.0.63/json/juzTranslations/juz_translation_1_132.json`

```json
{
    "Id": 132,
    "LanguageCodes": [
        "en",
        "eng"
    ],
    "Name": "eng-mustafakhattaba",
    "Author": "Mustafa Khattab Allah Edition",
    "Language": "English",
    "Description": "English",
    "Direction": "ltr",
    "Source": "",
    "Translations": [
        {
            "SurahNumber": 1,
            "VerseNumber": 1,
            "Text": "In the Name of Allah—the Most Compassionate, Most Merciful"
        },
        ...
```

### Pages

> Get Page info in English locale.
- **GET `/json/pages/en_pageInfo.json`**


  - `https://localhost:3123/json/pages/en_pageInfo.json`
  - `https://cdn.jsdelivr.net/gh/quranstatic/static@v0.0.63/json/pages/en_pageInfo.json`

```json
{
    "LanguageCode": "en",
    "PageInfos": [
        {
            "Page": 1,
            "ChapterStart": 1,
            "ChapterEnd": 1,
            "Text": "بِسۡمِ ٱللَّهِ ٱلرَّحۡمَٰنِ ٱلرَّحِيمِ ١ ٱلۡحَمۡدُ لِلَّهِ رَبِّ ٱلۡعَٰلَمِينَ ٢",
            "SurahVerseKeys": [
                "1:1",
                "1:2",
                "1:3",
                "1:4",
                "1:5",
                "1:6",
                "1:7"
            ]
        },
        ...
```

> Get Page data.
- **GET `/json/pages/page_{number}.json`**


  - `https://localhost:3123/json/pages/page_1.json`
  - `https://cdn.jsdelivr.net/gh/quranstatic/static@v0.0.63/json/pages/page_1.json`

```json
{
    "PageNumber": 1,
    "Verses": [
        {
            "SurahNumber": 1,
            "SurahIndex": "001",
            "VerseNumber": 1,
            "ShowSurahName": true,
            "ShowBismillah": false,
            "Text": "بِسۡمِ ٱللَّهِ ٱلرَّحۡمَٰنِ ٱلرَّحِيمِ",
            "TajweedText": "بِسْمِ <tajweed class=ham_wasl>ٱ</tajweed>للَّهِ <tajweed class=ham_wasl>ٱ</tajweed><tajweed class=laam_shamsiyah>ل</tajweed>رَّحْمَ<tajweed class=madda_normal>ـٰ</tajweed>نِ <tajweed class=ham_wasl>ٱ</tajweed><tajweed class=laam_shamsiyah>ل</tajweed>رَّح<tajweed class=madda_permissible>ِي</tajweed>مِ <span class=end>١</span>",
            "End": "١",
            "PageNumber": 1,
            ...
```

> Get Page word-by-word translation in English.
- **GET `/json/pages/page_wbw_en_{number}.json`**


  - `https://localhost:3123/json/pages/page_wbw_en_1.json`
  - `https://cdn.jsdelivr.net/gh/quranstatic/static@v0.0.63/json/pages/page_wbw_en_1.json`

```json
{
    "PageNumber": 1,
    "Verses": [
        {
            "SurahNumber": 1,
            "SurahIndex": "001",
            "VerseNumber": 1,
            "ShowSurahName": true,
            "ShowBismillah": false,
            "Words": [
                {
                    "Position": 1,
                    "AudioFileName": "001_001_001.mp3",
                    "TajweedImageFilePath": "1/1/1.png",
                    "Text": "بِسْمِ",
                    "Translation": "In (the) name",
                    "Transliteration": "bis'mi",
                    "Root": "س م و"
                },
                ...
```

> Get Page translation for a specific edition.
- **GET `/json/pageTranslations/page_translation_{number}_{edition}.json`**


  - `https://localhost:3123/json/pageTranslations/page_translation_1_132.json`
  - `https://cdn.jsdelivr.net/gh/quranstatic/static@v0.0.63/json/pageTranslations/page_translation_1_132.json`

```json
{
    "Id": 132,
    "LanguageCodes": [
        "en",
        "eng"
    ],
    "Name": "eng-mustafakhattaba",
    "Author": "Mustafa Khattab Allah Edition",
    "Language": "English",
    "Description": "English",
    "Direction": "ltr",
    "Source": "",
    "Translations": [
        {
            "SurahNumber": 1,
            "VerseNumber": 1,
            "Text": "In the Name of Allah—the Most Compassionate, Most Merciful"
        },
        ...
```

### Rukus

> Get Ruku info in English locale.
- **GET `/json/rukus/en_rukuInfo.json`**


  - `https://localhost:3123/json/rukus/en_rukuInfo.json`
  - `https://cdn.jsdelivr.net/gh/quranstatic/static@v0.0.63/json/rukus/en_rukuInfo.json`

```json
{
    "LanguageCode": "en",
    "RukuInfos": [
        {
            "Ruku": 1,
            "ChapterStart": 1,
            "ChapterEnd": 1,
            "Text": "بِسۡمِ ٱللَّهِ ٱلرَّحۡمَٰنِ ٱلرَّحِيمِ ١ ٱلۡحَمۡدُ لِلَّهِ رَبِّ ٱلۡعَٰلَمِينَ ٢",
            "SurahVerseKeys": [
                "1:1",
                "1:2",
                "1:3",
                "1:4",
                "1:5",
                "1:6",
                "1:7"
            ]
        },
        ...
```

> Get Ruku data.
- **GET `/json/rukus/ruku_{number}.json`**


  - `https://localhost:3123/json/rukus/ruku_1.json`
  - `https://cdn.jsdelivr.net/gh/quranstatic/static@v0.0.63/json/rukus/ruku_1.json`

```json
{
    "RukuNumber": 1,
    "Verses": [
        {
            "SurahNumber": 1,
            "SurahIndex": "001",
            "VerseNumber": 1,
            "ShowSurahName": true,
            "ShowBismillah": false,
            "Text": "بِسۡمِ ٱللَّهِ ٱلرَّحۡمَٰنِ ٱلرَّحِيمِ",
            "TajweedText": "بِسْمِ <tajweed class=ham_wasl>ٱ</tajweed>للَّهِ <tajweed class=ham_wasl>ٱ</tajweed><tajweed class=laam_shamsiyah>ل</tajweed>رَّحْمَ<tajweed class=madda_normal>ـٰ</tajweed>نِ <tajweed class=ham_wasl>ٱ</tajweed><tajweed class=laam_shamsiyah>ل</tajweed>رَّح<tajweed class=madda_permissible>ِي</tajweed>مِ <span class=end>١</span>",
            "End": "١",
            "PageNumber": 1,
            "JuzNumber": 1,
            "ManzilNumber": 1,
            "RukuNumber": 1,
            ...
```

> Get Ruku word-by-word translation in English.
- **GET `/json/rukus/ruku_wbw_en_{number}.json`**


  - `https://localhost:3123/json/rukus/ruku_wbw_en_1.json`
  - `https://cdn.jsdelivr.net/gh/quranstatic/static@v0.0.63/json/rukus/ruku_wbw_en_1.json`

```json
{
    "RukuNumber": 1,
    "Verses": [
        {
            "SurahNumber": 1,
            "SurahIndex": "001",
            "VerseNumber": 1,
            "ShowSurahName": true,
            "ShowBismillah": false,
            "Words": [
                {
                    "Position": 1,
                    "AudioFileName": "001_001_001.mp3",
                    "TajweedImageFilePath": "1/1/1.png",
                    "Text": "بِسْمِ",
                    "Translation": "In (the) name",
                    "Transliteration": "bis'mi",
                    "Root": "س م و"
                },
                ...
```

> Get Ruku translation for a specific edition.
- **GET `/json/rukuTranslations/ruku_translation_{number}_{edition}.json`**


  - `https://localhost:3123/json/rukuTranslations/ruku_translation_1_132.json`
  - `https://cdn.jsdelivr.net/gh/quranstatic/static@v0.0.63/json/rukuTranslations/ruku_translation_1_132.json`

```json
{
    "Id": 132,
    "LanguageCodes": [
        "en",
        "eng"
    ],
    "Name": "eng-mustafakhattaba",
    "Author": "Mustafa Khattab Allah Edition",
    "Language": "English",
    "Description": "English",
    "Direction": "ltr",
    "Source": "",
    "Translations": [
        {
            "SurahNumber": 1,
            "VerseNumber": 1,
            "Text": "In the Name of Allah—the Most Compassionate, Most Merciful"
        },
        ...
```

### Maqra

> Get Maqra info in English locale.
- **GET `/json/maqras/en_maqraInfo.json`**


  - `https://localhost:3123/json/maqras/en_maqraInfo.json`
  - `https://cdn.jsdelivr.net/gh/quranstatic/static@v0.0.63/json/maqras/en_maqraInfo.json`

```json
{
    "LanguageCode": "en",
    "MaqraInfos": [
        {
            "Maqra": 1,
            "HizbNoDecimal": 1,
            "HizbDecimalOnly": 0.0,
            "HizbWithDecimal": 1.0,
            "HizbFractionOnly": "",
            "HizbWithFraction": "1",
            "ChapterStart": 1,
            "ChapterEnd": 2,
            "Text": "بِسۡمِ ٱللَّهِ ٱلرَّحۡمَٰنِ ٱلرَّحِيمِ ١ ٱلۡحَمۡدُ لِلَّهِ رَبِّ ٱلۡعَٰلَمِينَ ٢",
            "SurahVerseKeys": [
                "1:1",
                "1:2",
                "1:3",
                "1:4",
                "1:5",
                ...
```

> Get Maqra data.
- **GET `/json/maqras/maqra_{number}.json`**


  - `https://localhost:3123/json/maqras/maqra_1.json`
  - `https://cdn.jsdelivr.net/gh/quranstatic/static@v0.0.63/json/maqras/maqra_1.json`

```json
{
    "MaqraNumber": 1.0,
    "Verses": [
        {
            "SurahNumber": 1,
            "SurahIndex": "001",
            "VerseNumber": 1,
            "ShowSurahName": true,
            "ShowBismillah": false,
            "Text": "بِسۡمِ ٱللَّهِ ٱلرَّحۡمَٰنِ ٱلرَّحِيمِ",
            "TajweedText": "بِسْمِ <tajweed class=ham_wasl>ٱ</tajweed>للَّهِ <tajweed class=ham_wasl>ٱ</tajweed><tajweed class=laam_shamsiyah>ل</tajweed>رَّحْمَ<tajweed class=madda_normal>ـٰ</tajweed>نِ <tajweed class=ham_wasl>ٱ</tajweed><tajweed class=laam_shamsiyah>ل</tajweed>رَّح<tajweed class=madda_permissible>ِي</tajweed>مِ <span class=end>١</span>",
            "End": "١",
            "PageNumber": 1,
            "JuzNumber": 1,
            "ManzilNumber": 1,
            "RukuNumber": 1,
            ...
```

> Get Maqra word-by-word translation in English.
- **GET `/json/maqras/maqra_wbw_en_{number}.json`**


  - `https://localhost:3123/json/maqras/maqra_wbw_en_1.json`
  - `https://cdn.jsdelivr.net/gh/quranstatic/static@v0.0.63/json/maqras/maqra_wbw_en_1.json`

```json
{
    "MaqraNumber": 1,
    "Verses": [
        {
            "SurahNumber": 1,
            "SurahIndex": "001",
            "VerseNumber": 1,
            "ShowSurahName": true,
            "ShowBismillah": false,
            "Words": [
                {
                    "Position": 1,
                    "AudioFileName": "001_001_001.mp3",
                    "TajweedImageFilePath": "1/1/1.png",
                    "Text": "بِسْمِ",
                    "Translation": "In (the) name",
                    "Transliteration": "bis'mi",
                    "Root": "س م و"
                },
                ...
```

> Get Maqra translation for a specific edition.
- **GET `/json/maqraTranslations/maqra_translation_{number}_{edition}.json`**


  - `https://localhost:3123/json/maqraTranslations/maqra_translation_1_132.json`
  - `https://cdn.jsdelivr.net/gh/quranstatic/static@v0.0.63/json/maqraTranslations/maqra_translation_1_132.json`

```json
{
    "Id": 132,
    "LanguageCodes": [
        "en",
        "eng"
    ],
    "Name": "eng-mustafakhattaba",
    "Author": "Mustafa Khattab Allah Edition",
    "Language": "English",
    "Description": "English",
    "Direction": "ltr",
    "Source": "",
    "Translations": [
        {
            "SurahNumber": 1,
            "VerseNumber": 1,
            "Text": "In the Name of Allah—the Most Compassionate, Most Merciful"
        },
        ...
```

### Memorise

> Get Memorise courses in English locale.
- **GET `/json/memorise/en_courses.json`**


  - `https://localhost:3123/json/memorise/en_courses.json`
  - `https://cdn.jsdelivr.net/gh/quranstatic/static@v0.0.63/json/memorise/en_courses.json`

```json
[
    {
        "Id": 1,
        "Name": "Quranic Vocabulary",
        "Description": "Learn Quranic vocabulary with reference to verses",
        "Units": [
            {
                "Id": 1,
                "Name": "1 - 100",
                "ReviewCount": 100,
                "QuestionCount": 200
            },
            {
                "Id": 2,
                "Name": "101 - 200",
                "ReviewCount": 100,
                "QuestionCount": 200
            },
            ...
```

> Get Memorise questions for course 2, lesson 3.
- **GET `/json/memorise/en_questions_2_3.json`**


  - `https://localhost:3123/json/memorise/en_questions_2_3.json`
  - `https://cdn.jsdelivr.net/gh/quranstatic/static@v0.0.63/json/memorise/en_questions_2_3.json`

```json
[
    {
        "Id": "5AD1B926654EC4446BF309A8CE152801B813993581BB3BDF256EE06D8CEC9404",
        "Category": 2,
        "CategoryType": 2,
        "ReviewSourceValue": null,
        "ReviewDestinationValue": null,
        "Question": {
            "Type": "text",
            "Value": "Yes/ No Question",
            "TextDirection": "ltr",
            "IsBigger": false,
            "AudioFileName": "B1F70852B3C464A51A373B363EFDF53F04CC6B4A248B9F8C1FDE8F36B6E0909C.mp3",
            "Choices": [
                "(أَلَّا (أَنْ+لَا",
                "هؤُلَآءِ",
                "هذِهِ",
                "هَـذِهِ",
                "لَيْس/ لَيْسَت",
                ...
```

> Get Memorise reviews for course 2, lesson 3.
- **GET `/json/memorise/en_reviews_2_3.json`**


  - `https://localhost:3123/json/memorise/en_reviews_2_3.json`
  - `https://cdn.jsdelivr.net/gh/quranstatic/static@v0.0.63/json/memorise/en_reviews_2_3.json`

```json
[
    {
        "Id": "5AD1B926654EC4446BF309A8CE152801B813993581BB3BDF256EE06D8CEC9404",
        "Category": 1,
        "CategoryType": 1,
        "ReviewSourceValue": {
            "Type": "text",
            "Value": "أَ / هَلۡ",
            "TextDirection": "rtl",
            "IsBigger": false
        },
        "ReviewDestinationValue": {
            "Type": "text",
            "Value": "Yes/ No Question",
            "TextDirection": "ltr",
            "IsBigger": false
        },
        "Question": null,
        "Answer": null,
        ...
```

### NamesOfAllah

> Get Names of Allah in English locale.
- **GET `/json/namesOfAllah/en_names.json`**


  - `https://localhost:3123/json/namesOfAllah/en_names.json`
  - `https://cdn.jsdelivr.net/gh/quranstatic/static@v0.0.63/json/namesOfAllah/en_names.json`

```json
[
    {
        "LanguageCode": "en",
        "Number": 1,
        "Transliteration": "Ar-Rahman",
        "ImageFileName": "1.png",
        "AudioFileName": "1.mp3",
        "Names": [
            "The Exceedingly Compassionate",
            "The Most Merciful",
            "The All-Merciful",
            "The Most or Entirely Merciful",
            "The Beneficent"
        ],
        "Descriptions": [
            "Allah, Ar-Rahmaan, bestows His Mercy (Rahmah) upon all the creatures in this universe.",
            "He who wills goodness and mercy for all His creatures."
        ]
    },
    ...
```

### Prophets

> Get Prophets in English locale.
- **GET `/json/prophets/en_prophets.json`**


  - `https://localhost:3123/json/prophets/en_prophets.json`
  - `https://cdn.jsdelivr.net/gh/quranstatic/static@v0.0.63/json/prophets/en_prophets.json`

```json
[
    {
        "Id": 1,
        "LanguageCode": "en",
        "TranslatedName": "Adam",
        "ArabicName": "آدم",
        "ArabicTransliteratedName": "Adam",
        "AudioFileName": "1.mp3",
        "SourceHtml": "<body>\r\n    <p>The story of <strong>Prophet Adam (peace be upon him)</strong> is a foundational
        ...
```

### Reciters

> Get Reciter info in English locale.
- **GET `/json/reciterInfos/en_{reciter}.json`**


  - `https://localhost:3123/json/reciterInfos/en_abderahmane-eloosi-1.json`
  - `https://cdn.jsdelivr.net/gh/quranstatic/static@v0.0.63/json/reciterInfos/en_abderahmane-eloosi-1.json`

```json
{
    "LanguageCode": "en",
    "Id": "abderahmane-eloosi-1",
    "Name": "Abderahmane Eloosi",
    "ImagePath": "images/reciters/abderahmane-eloosi-1.png",
    "BioText": "<h2>Early Life and Education</h2>\r\n    <p>\r\n        Abderahmane Eloosi, renowned for his exceptional
    ...
```

> Get Reciters list in English locale.
- **GET `/json/reciters/en_reciters.json`**


  - `https://localhost:3123/json/reciters/en_reciters.json`
  - `https://cdn.jsdelivr.net/gh/quranstatic/static@v0.0.63/json/reciters/en_reciters.json`

```json
[
    {
        "LanguageCode": "en",
        "Id": "abdulbaset-mujawwad-2",
        "Name": "Abd Al-Basit Mujawwad",
        "ImagePath": "images/reciters/abdulbaset-mujawwad-2.png",
        "BioText": "<p>‘Abdul-Basit ‘Abdus-Samad (Arabic: عبـدُ الباسِـط مُحـمّـد عبـدُ ٱلصّـمـد), or Abdel Basit Abdel Samad,
        ...
```

### Root Words

> Get Root Words in English locale.
- **GET `/json/rootWords/en_rootWords.json`**


  - `https://localhost:3123/json/rootWords/en_rootWords.json`
  - `https://cdn.jsdelivr.net/gh/quranstatic/static@v0.0.63/json/rootWords/en_rootWords.json`

```json
{
    "س م و": {
        "Summary": "in the name; the sky; and the sky; heavens; the names; of the names; of their names; of the heavens; his name; fixed; [I] have named her; the name; names; you have named them; his names; and o sky; name them; and the heavens; this name; named you; heaven; surely they name; named",
        "VerbalNoun": "تَسْمِيَةً",
        "PassiveParticle": "مُسَمًّى",
        "ActiveParticle": "مُسَمٍّ",
        "Imperative": "سَمِّ",
        "Imperfect": "يُسَمِّي",
        "Perfect": "سَمّٰى",
        "DerivedWords": [
            [
                {
                    "K": "1:1:1",
                    "W": "بِسْمِ",
                    "T": "in the name"
                }
            ],
            ...
```

### Search

> Get Search index in English locale.
- **GET `/json/search/en_search.json`**


  - `https://localhost:3123/json/search/en_search.json`
  - `https://cdn.jsdelivr.net/gh/quranstatic/static@v0.0.63/json/search/en_search.json`

```json
{
    "aad": [
        "11:50",
        "11:59",
        "11:60",
        "14:9",
        "22:42",
        "25:38",
        "29:38",
        "38:12",
        "40:31",
        "41:13",
        "41:15",
        "46:21",
        "69:4",
        "69:6",
        "89:6"
    ],
    "aaron": [
        "2:248",
        "4:163",
        "6:84",
        ...
```

### Tafsirs

> Get Tafsir for a Surah and edition.
- **GET `/json/tafsirs/tafsir_{surah}_{edition}.json`**


  - `https://localhost:3123/json/tafsirs/tafsir_1_14.json`
  - `https://cdn.jsdelivr.net/gh/quranstatic/static@v0.0.63/json/tafsirs/tafsir_1_14.json`

```json
{
    "Id": 14,
    "LanguageCodes": [
        "ar",
        "ara"
    ],
    "Name": "Tafsir Ibn Kathir",
    "Author": "Hafiz Ibn Kathir",
    "Language": "arabic",
    "Direction": "rtl",
    "Source": "https://quran.com/",
    "Tafsirs": [
        {
            "SurahNumber": 1,
            "VerseNumber": 1,
            "Text": "بسم الله الرحمن الرحيم سورة الفاتحة . يقال لها الفاتحة أي فاتحة الكتاب خطا وبها تفتح القراءة في الصلوات
            ...
```

### Verses

> Get all verses in Arabic.
- **GET `/json/verses/verses.json`**


  - `https://localhost:3123/json/verses/verses.json`
  - `https://cdn.jsdelivr.net/gh/quranstatic/static@v0.0.63/json/verses/verses.json`

```json
{
    "1:1": "بِسۡمِ ٱللَّهِ ٱلرَّحۡمَٰنِ ٱلرَّحِيمِ",
    "1:2": "ٱلۡحَمۡدُ لِلَّهِ رَبِّ ٱلۡعَٰلَمِينَ",
    "1:3": "ٱلرَّحۡمَٰنِ ٱلرَّحِيمِ",
    "1:4": "مَٰلِكِ يَوۡمِ ٱلدِّينِ",
    ...
```

> Get all verses in English locale.
- **GET `/json/verses/en_verses.json`**


  - `https://localhost:3123/json/verses/en_verses.json`
  - `https://cdn.jsdelivr.net/gh/quranstatic/static@v0.0.63/json/verses/en_verses.json`

```json
{
    "1:1": "In the name of God, the Lord of Mercy, the Giver of Mercy",
    "1:2": "Praise belongs to God, Lord of the Worlds",
    "1:3": "the Lord of Mercy, the Giver of Mercy",
    "1:4": "Master of the Day of Judgement",
    "1:5": "It is You we worship; it is You we ask for help",
    "1:6": "Guide us to the straight path",
    ...
```

### Health & Diagnostics

> Get health status.
- **GET `/health`**


  - `https://localhost:3123/health`

### OpenAPI / Swagger

> Get the Swagger UI
- **GET `/swagger`**


  - `https://localhost:3123/swagger/index.html`


> Get OpenAPI specification for MemQuran API.
- **GET `/openapi/memquranapi.json`**


  - `https://localhost:3123/openapi/memquranapi.json`

---

## Error Handling

Standard error format:

```json
{
  "type": "Exception",
  "title": "An error occurred",
  "status": 500,
  "detail": "An error occurred in our API. Use the trace id when contacting us.",
  "traceId": "00-2cf649405a126795be46fd7c8319813f-75cf8e846bd3acc7-01",
  "instance": "GET /json/surahs/surah_1.json",
  "errorMessage": "Exception of type 'System.Exception' was thrown.",
  "stackTrace": "..."
}
```

Common status codes:

- 200 OK
- 400 Bad Request (validation)
- 404 Not Found
- 500 Internal Server Error

---

## Observability & Diagnostics

- **OpenTelemetry**  
  Configure an OTLP collector via the `OTEL_EXPORTER_OTLP_ENDPOINT` environment variable.
- **Seq logging**  
  Provide server URL and API key in configuration to ship structured logs.
- **Redis cache**  
  Provide a Redis connection string in configuration to enable distributed cache and health checks.

---

## Project Layout

- API: `src/MemQuran.Api/MemQuran.Api.csproj`
- Core domain: `src/MemQuran.Core`
- Infrastructure: `src/MemQuran.Infrastructure`

---

## Troubleshooting

- Ensure all environment variables and configuration files are set correctly.
- Check logs (Seq, console) for errors.
- Use `/health` for diagnostics.
- For Redis/OTLP/Seq, verify connectivity and credentials.

---

## Contributing

Contributions are welcome! Please open issues or submit pull requests.

---

## License

MIT
