# DonationMatchingSystem Frontend

ממשק React בעברית וב־RTL עבור תורמים, נתרמים ומנהלי מערכת.

## הרצה מקומית

נדרש Node.js 22 ו־Corepack.

```bash
corepack enable
pnpm install
cp .env.example .env.local
pnpm dev
```

ברירת המחדל מפנה את `/api` אל `http://localhost:5255` באמצעות Vite proxy. ניתן לשנות זאת באמצעות `VITE_DEV_API_TARGET`.

## בדיקות

```bash
pnpm lint
pnpm typecheck
pnpm test
pnpm build
pnpm e2e
```

## מגבלות ידועות

ה־Backend אינו מספק כרגע endpoint ציבורי לסוגי אוכלוסייה. לכן רישום נתרם ויצירת תרומה חסומים באופן מפורש ב־production, ולא נעשה שימוש במזהים קשיחים. עריכת תרומה קיימת ממשיכה לעבוד. פרטים נוספים ב־`BACKEND_GAPS.md`.
