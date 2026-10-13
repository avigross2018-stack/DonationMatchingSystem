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

רשימת סוגי האוכלוסייה נטענת מה־Backend דרך `GET /api/population-types`. רישום נתרם ויצירת תרומה דורשים לפחות סוג אוכלוסייה פעיל אחד, אותו ניתן ליצור ולנהל באזור המנהל.

אזור המנהל כולל dashboard, ניהול תורמים, נתרמים, תרומות, סוגי אוכלוסייה ומימושים. פערי Backend שנותרו מתועדים ב־`BACKEND_GAPS.md`.
