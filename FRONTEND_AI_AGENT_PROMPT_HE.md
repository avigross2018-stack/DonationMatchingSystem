# פרומפט לסוכן AI — פיתוח Frontend עבור DonationMatchingSystem

אתה סוכן פיתוח בכיר שאחראי לתכנן ולממש Frontend מלא, איכותי וניתן לתחזוקה עבור מערכת `DonationMatchingSystem`. עליך לעבוד באופן עצמאי עד לתוצר עובד, אך לא להמציא חוזי API ולא לשנות את ה־Backend בלי אישור מפורש.

## 1. מטרת המשימה

בנה אפליקציית Web מודרנית, רספונסיבית ונגישה בעברית, בכיוון `RTL`, עבור שלושה תפקידים:

1. תורם (`DONOR`)
2. נתרם (`RECIPIENT`)
3. מנהל מערכת (`ADMIN`)

ה־Backend עדיין בפיתוח. לכן עליך:

- לעבוד מול ה־API הקיים בפועל בענף `dev`.
- לזהות במדויק אילו יכולות קיימות, חלקיות או חסרות.
- לממש באופן מלא את הזרימות שכבר נתמכות.
- להכין ארכיטקטורה ומסכים ליכולות עתידיות, אך לא להציג נתוני mock כאילו הגיעו משרת אמיתי.
- לבודד יכולות חסרות באמצעות feature flags, API adapters ומצב mock מפורש לפיתוח בלבד.
- לייצר מסמך פערים ברור עבור צוות ה־Backend.

אל תסתפק בתוכנית, wireframe או שלד. בסיום נדרש Frontend עובד, בדיקות, תיעוד והוראות הרצה.

## 2. מקורות אמת וסדר עדיפות

עבוד מול המאגר:

`https://github.com/avigross2018-stack/DonationMatchingSystem/tree/dev`

נקודת הייחוס בעת כתיבת הפרומפט היא ענף `dev`, commit:

`e4796d63c1806e0d6e1814a7f389abead117b51c`

לפני תחילת העבודה משוך או קרא את מצב הענף העדכני ובדוק אם ה־commit השתנה. סדר העדיפות במקרה של סתירה:

1. Controllers, DTOs, Swagger והקוד בפועל ב־Backend.
2. בדיקות ה־API הקיימות.
3. מסמכי handoff ואפיון.
4. ההנחות בפרומפט הזה.

אל תניח שהמסמך מעודכן יותר מהקוד. אל תמציא endpoint, field, status או הרשאה. אם קיים פער, תעד אותו ב־`BACKEND_GAPS.md` והגן על ה־UI בהתאם.

## 3. גבולות העבודה

- ה־Frontend הוא פרויקט נפרד מה־ASP.NET Core API, אך יישמר באותו repository, בתיקייה ברורה כגון `src/Frontend`.
- אין לבצע EF migrations, אין לשנות סכמת MySQL ואין לגעת בסקריפט יצירת הסכמה.
- אין לשנות לוגיקה עסקית ב־Backend במסגרת משימת Frontend.
- אם נדרש שינוי Backend כדי לאפשר פעולה אמיתית, אל תבצע אותו אוטומטית. תעד את הצורך, את חוזה ה־API המוצע ואת ההשפעה.
- אל תחשוף secrets, JWT, סיסמאות או פרטים אישיים בלוגים, ב־UI או ב־repository.
- אל תדחוף ישירות לענף `dev`. עבוד בענף feature נפרד אם סביבת העבודה מאפשרת זאת.
- שמור שינויים קיימים של משתמשים אחרים ואל תדרוס אותם.

## 4. טכנולוגיות נדרשות

השתמש בברירת המחדל הבאה, אלא אם כבר קיים Frontend עם stack אחר שיש לכבד:

- React 19
- TypeScript במצב strict
- Vite
- React Router
- TanStack Query לניהול server state, caching ו־mutations
- React Hook Form יחד עם Zod עבור טפסים וולידציה
- MUI עם theme מותאם, RTL ו־Emotion RTL cache
- Axios או wrapper קטן מעל `fetch`, עם client מרכזי אחד בלבד
- Vitest + React Testing Library
- Playwright לבדיקות E2E קריטיות
- ESLint + Prettier

אל תוסיף state-management גלובלי כבד ללא צורך. authentication state קטן יכול להיות ב־Context או store מינימלי. מידע מהשרת ינוהל דרך TanStack Query ולא יוכפל ב־global store.

## 5. שפה, עיצוב וחוויית משתמש

- כל הממשק למשתמש יהיה בעברית ובכיוון `RTL`.
- הקוד, שמות משתנים, types, tests ו־commits יהיו באנגלית.
- השתמש בפורמט כספי `he-IL` ובמטבע שקל: `Intl.NumberFormat('he-IL', { style: 'currency', currency: 'ILS' })`.
- הצג תאריכים בפורמט מקומי ברור, אך שלח ל־API ערכי ISO 8601.
- העיצוב צריך לשדר אמינות, שקיפות ורוגע, ולא להיראות כמו template גנרי.
- צבעים מומלצים: כחול/טורקיז לאמון, ירוק לפעולה חיובית, כתום לאזהרות, אדום לפעולות הרסניות.
- נדרשת רספונסיביות מלאה: mobile, tablet ו־desktop.
- הוסף loading states, skeletons, empty states, error states ו־success feedback בכל זרימה אסינכרונית.
- פעולות הרסניות או סופיות יחייבו dialog אישור ברור.
- שמור focus תקין, labels מלאים, keyboard navigation, contrast תקין ו־ARIA היכן שנדרש. שאף ל־WCAG 2.1 AA.
- אין להציג מידע פנימי כמו stack traces או `traceId` למשתמש הרגיל. אפשר לאפשר העתקת `traceId` בפרטי שגיאה טכניים מוסתרים לצורכי תמיכה.

## 6. הבנת המערכת העסקית

זו מערכת סימולציה להתאמת תרומות; אין סליקה או כסף אמיתי.

- תורם נרשם, מנהל פרופיל ויוצר תרומות לסוג אוכלוסייה.
- נתרם נרשם עם סוג אוכלוסייה, הכנסה, מספר ילדים וסיבת בקשה.
- השרת מחשב זכאות ויתרת זכאות.
- מנגנון רקע יוצר התאמות מסוג `Redemption` במצב `PENDING`.
- מנהל מאשר או דוחה התאמה.
- רק אישור מנהל מפחית אטומית את יתרת התרומה ואת יתרת הזכאות.

נוסחת הזכאות נמצאת באחריות השרת:

`max(0, BaseEligibilityAmount + AmountOfChildren * 500 - IncomeAmount * 0.20)`

מותר להציג הסבר כללי למשתמש, אך אסור לחשב בצד הלקוח ערך “מחייב”. לאחר רישום או עדכון יש להציג את הערך שהוחזר מהשרת בלבד.

סטטוסים מרכזיים:

- תפקידים: `DONOR`, `RECIPIENT`, `ADMIN`
- תרומה: `ACTIVE`, `COMPLETED`, `CANCELLED`, `EXPIRED`
- מימוש: `PENDING`, `COMPLETED`, `CANCELLED`, `REJECTED`

צור mapping מרכזי בין הערכים מהשרת לבין labels וצבעים בעברית. אל תפזר מחרוזות סטטוס בקומפוננטות.

חשוב במיוחד: `PENDING` הוא הצעה בלבד ואינו שומר או משריין יתרות. לכן במסך מנהל יש להציג שהסכום עשוי להשתנות בזמן האישור. אישור יכול להחזיר `409 Conflict` אם אין עוד סכום זמין או אם הרשומה כבר טופלה. במקרה כזה אין להציג success אופטימי; יש לרענן את הפרטים והרשימה ולהסביר שהנתונים השתנו.

## 7. חוזה authentication והרשאות

### Login

`POST /api/Auth/login`

Request:

```json
{
  "phone": "string",
  "password": "string"
}
```

Success `200`:

```json
{
  "accessToken": "jwt",
  "tokenType": "Bearer"
}
```

Failure `401` עשוי להגיע כ־`{ "message": "Invalid phone or password" }`.

ה־JWT כולל:

- `sub`: מזהה `User.Id`
- role: אחד מהתפקידים `DONOR`, `RECIPIENT`, `ADMIN`; בפענוח token יש לתמוך בשם ה־claim שנוצר על ידי .NET עבור `ClaimTypes.Role` וכן ב־`role` אם serialization עתידי ישתנה.
- `exp`: מועד תפוגה

### דרישות authentication

- כל בקשה מוגנת תשלח `Authorization: Bearer <token>`.
- צור route guards לפי authenticated state ולפי role.
- משתמש מחובר שייכנס ל־`/login` יופנה ל־dashboard המתאים לתפקידו.
- משתמש ללא הרשאה יראה מסך 403 ידידותי ולא תוכן מהבהב של תפקיד אחר.
- כאשר token פג או שהשרת מחזיר 401, נקה session, בטל queries רגישות והעבר ל־login עם הודעה מתאימה.
- אין refresh-token endpoint. אל תמציא refresh flow.
- אין endpoint של logout. logout הוא מחיקת session בצד הלקוח.
- אל תציג או תרשום את ה־JWT ל־console.
- העדף token בזיכרון עם persistence מצומצם ומודע לסיכון. אם נדרשת הישארות לאחר refresh, השתמש ב־`sessionStorage` כברירת מחדל ותעד את ההחלטה. אין לשמור token ב־localStorage בלי הצדקה מפורשת.
- client-side authorization משפר UX בלבד; השרת נשאר מקור האמת להרשאות.

## 8. חוזי API קיימים שחייבים לעבוד

Base URL יגיע מ־`VITE_API_BASE_URL`. בסביבת פיתוח ניתן להשתמש ב־Vite proxy כדי להימנע מבעיות CORS. כתובת הפיתוח הנוכחית של ה־Backend היא בדרך כלל `http://localhost:5255` או `https://localhost:7005`.

### ציבורי

- `POST /api/Auth/register/donor`
- `POST /api/Auth/register/recipient`
- `POST /api/Auth/login`

רישום תורם:

```ts
type RegisterDonorRequest = {
  name: string;       // required, max 50
  phone: string;      // required, phone, max 50
  password: string;   // required, min 8
};
```

רישום נתרם:

```ts
type RegisterRecipientRequest = {
  name: string;
  phone: string;
  password: string;
  populationTypeId: number; // positive
  incomeAmount: number;      // 0..99,999,999.99
  amountOfChildren: number;  // non-negative integer
  requestReason?: string | null; // max 250
};
```

Success ברישום הוא `201` עם `message` ו־`userId`. אין login אוטומטי מובטח; לאחר הצלחה העבר למסך login עם הודעת הצלחה.

### לכל משתמש מאומת

- `PUT /api/Auth/change-password` — request: `{ currentPassword, newPassword }`, הצלחה `204`.

### תורם

- `GET /api/donors/me`
- `PUT /api/donors/me` — `{ name, phone }`, הצלחה `204`
- `DELETE /api/donors/me` — השבתה, הצלחה `204`
- `POST /api/donations`
- `GET /api/donations/show-donations`
- `PUT /api/donations/{id}`
- `POST /api/donations/{id}/cancel`

DTO ליצירת תרומה:

```ts
type CreateDonationRequest = {
  populationTypeId: number;
  totalAmount: number;              // 0.01..99,999,999.99
  maxAmountPerRecipient: number;    // 0.01..99,999,999.99
  description: string;              // max 250
  expirationDate: string;           // ISO date-time, future
};
```

DTO לעדכון תרומה זהה ללא `populationTypeId`. הצלחה בעדכון ובביטול היא `204`.

תרומה מוחזרת עם:

```ts
type Donation = {
  id: number;
  populationTypeId: number;
  totalAmount: number;
  remainingAmount: number;
  maxAmountPerRecipient: number;
  description?: string | null;
  createdAt: string;
  expirationDate?: string | null;
  status: 'ACTIVE' | 'COMPLETED' | 'CANCELLED' | 'EXPIRED' | string;
};
```

### נתרם

- `GET /api/recipients/me`
- `PUT /api/recipients/me` — `{ incomeAmount, amountOfChildren, requestReason }`, הצלחה `204`
- `GET /api/recipients/me/redemptions`
- `DELETE /api/recipients/me` — השבתה, הצלחה `204`

פרופיל נתרם כולל `id`, `name`, `phone`, `populationTypeId`, `populationTypeName`, `incomeAmount`, `amountOfChildren`, `requestReason`, `eligibilityAmount`, `remainingEligibility`.

מימוש לנתרם כולל `id`, `donationId`, `amount`, `status`, `donationDescription`, `createdAt`, `redeemedAt`.

שים לב: ה־endpoint לעדכון נתרם אינו מקבל `name`, `phone` או `populationTypeId`. אל תציג שדות אלה כניתנים לעריכה.

### מנהל

- `GET /api/admin/me`
- `GET /api/admin/redemptions`
- `GET /api/admin/redemptions/pending`
- `GET /api/admin/redemptions/{id}`
- `POST /api/admin/redemptions/{id}/approve`
- `POST /api/admin/redemptions/{id}/reject`

Query params לרשימת מימושים:

- `populationTypeId?: number`
- `status?: string`
- `fromDate?: ISO date-time`
- `toDate?: ISO date-time`
- `page: number` — מתחיל ב־1
- `pageSize: number` — 1 עד 100, ברירת מחדל 50

מבנה עמוד:

```ts
type Page<T> = {
  items: T[];
  page: number;
  pageSize: number;
  totalCount: number;
  totalPages: number;
};
```

פרטי מימוש מנהל כוללים מידע מלא על הנתרם, התרומה והתורם. קרא את `AdminRedemptionDto` בפועל וייצר types תואמים, ללא `any`.

אישור מחזיר בין היתר `redemptionId`, `status`, `approvedAmount`, `handledByAdminId`, `redeemedAt`, `donationRemainingAmount`, `recipientRemainingEligibility`.

דחייה מחזירה `redemptionId`, `status`, `handledByAdminId`.

## 9. חוזה שגיאות

השרת אינו מחזיר envelope אחיד בכל המקרים. בנה error normalizer מרכזי שתומך לפחות ב:

1. RFC Problem Details: `status`, `title`, `detail`, `instance`, `traceId`.
2. `ValidationProblemDetails` עם dictionary של `errors`.
3. `{ message: string }` עבור login.
4. גוף ריק ב־401/403.
5. שגיאת רשת או timeout.

מפה שגיאות ל־UX:

- `400`: הצג validation/business message ליד הטופס ובסיכום.
- `401`: סיים session והעבר ל־login, למעט login כושל שבו נשארים בטופס.
- `403`: מסך אין הרשאה או הודעה שהחשבון אינו פעיל.
- `404`: מסך לא נמצא או הודעה שהרשומה כבר לא קיימת.
- `409`: רענן data והצג conflict ידידותי; חשוב במיוחד באישור/דחיית מימוש.
- `500`: הודעה כללית ללא חשיפת פרטים פנימיים.

## 10. מסכים וזרימות למימוש

### אזור ציבורי

- דף בית קצר שמסביר שמדובר במערכת סימולציה ללא כסף אמיתי.
- Login לפי טלפון וסיסמה.
- בחירה בין רישום תורם לרישום נתרם.
- רישום תורם.
- רישום נתרם, כולל הסבר על הזכאות ושדות מדויקים.
- מסכי 404, 403 ושגיאה כללית.

### אזור תורם

- dashboard עם סיכום תרומות: מספר תרומות, סכום כולל, יתרה זמינה והתפלגות סטטוסים. כל עוד אין endpoint סטטיסטיקות, חשב רק מהמערך שהוחזר מ־`show-donations`, ואל תטען שזה נתון מערכתי מלא.
- רשימת תרומות עם cards במובייל וטבלה נגישה בדסקטופ.
- סינון וחיפוש מקומיים לפי סטטוס/תיאור/תאריך, כל עוד השרת אינו תומך בסינון.
- יצירת תרומה.
- עריכת תרומה פעילה בלבד.
- ביטול תרומה עם confirmation dialog והסבר שהפעולה מבטלת פעילות עתידית אך אינה מוחקת היסטוריה.
- פרופיל תורם, עריכת שם וטלפון.
- שינוי סיסמה.
- השבתת חשבון עם אישור מחמיר, logout אוטומטי לאחר הצלחה ואזהרה שהתרומות הפעילות יבוטלו.

### אזור נתרם

- dashboard המציג זכאות כוללת, יתרת זכאות ומימושים לפי סטטוס.
- profile summary עם סוג אוכלוסייה read-only.
- עדכון הכנסה, מספר ילדים וסיבת בקשה בלבד.
- לאחר עדכון, invalidation וטעינה מחדש של הפרופיל כדי להציג זכאות שחושבה בשרת.
- היסטוריית מימושים עם timeline או טבלה, status chips ופרטי תרומה זמינים.
- הסבר ש־`PENDING` אינו אישור סופי.
- שינוי סיסמה.
- השבתת חשבון עם אישור מחמיר והסבר שמימושים ממתינים יבוטלו.

### אזור מנהל — נתמך כעת

- dashboard בסיסי שמבוסס רק על endpoints קיימים: מימושים ממתינים, קישורים מהירים ופרופיל המנהל. אין להמציא סטטיסטיקות משתמשים או תרומות.
- רשימת כל המימושים עם server-side pagination ו־URL search params.
- רשימת מימושים ממתינים.
- סינון לפי סטטוס, טווח תאריכים וסוג אוכלוסייה כאשר קיים מקור אמין לרשימת האוכלוסיות.
- מסך פרטי מימוש עם פרטי נתרם, תרומה ותורם.
- אישור מימוש ב־dialog שמציג את הסכום המוצע ומדגיש שהשרת יחשב מחדש את הסכום המאושר.
- דחיית מימוש ב־dialog נפרד, ללא שינוי יתרות.
- מניעת double submit, disabled state במהלך mutation ו־refetch לאחר הצלחה או conflict.
- פרופיל מנהל read-only ושינוי סיסמה.

### אזור מנהל — הכנה עתידית בלבד

יש להכין routing, navigation, API interfaces ו־feature flags עבור:

- ניהול תורמים ונתרמים.
- ניהול תרומות.
- ניהול סוגי אוכלוסייה.
- dashboard סטטיסטיקות מלא.

כל עוד endpoints אלה אינם קיימים, אל תציג מסכים כאילו הם פעילים. אפשר להסתיר אותם או להציג “בפיתוח” רק במצב פיתוח. אין לבצע קריאות לנתיבים מומצאים.

## 11. פערי Backend שחייבים לקבל טיפול מפורש

בעת כתיבת הפרומפט זוהו הפערים הבאים:

1. אין endpoint ציבורי לקבלת סוגי אוכלוסייה פעילים. הדבר חוסם dropdown אמיתי ברישום נתרם וביצירת תרומה. אין להקשיח IDs בקוד production. הצע חוזה כגון `GET /api/population-types/active`, אבל אל תממש Backend ללא אישור.
2. אין CORS configuration ב־`Program.cs`. בפיתוח השתמש ב־Vite proxy. לפריסה תעד צורך ב־CORS מוגבל ל־origin ידוע או reverse proxy באותו origin.
3. אין endpoints מנהל לניהול משתמשים, תרומות וסוגי אוכלוסייה ואין endpoint סטטיסטיקות.
4. אין endpoint לקבלת תרומה בודדת לתורם ואין pagination לרשימת תרומות.
5. אין endpoint לתורם לצפייה במימושים שנוצרו מתרומותיו.
6. error contract אינו אחיד.
7. אין refresh token או endpoint logout.

צור `BACKEND_GAPS.md` עם: capability, impact, proposed endpoint, request/response example, authorization, priority ו־frontend fallback. אל תכלול סודות או מידע אישי בדוגמאות.

במצב `VITE_USE_MOCKS=true` מותר להפעיל MSW עם נתוני demo מסומנים היטב, כדי לאפשר פיתוח ו־E2E של מסכים עתידיים. ברירת המחדל production תהיה `false`, ו־build production לא יכלול או יפעיל mock handlers.

## 12. ארכיטקטורת Frontend רצויה

ארגן לפי features ולא לפי סוגי קבצים בלבד. מבנה לדוגמה:

```text
src/Frontend/
  src/
    app/                 # bootstrap, router, providers, theme
    api/                 # base client, error normalization, generated/manual contracts
    auth/                # session, JWT parsing, guards
    features/
      auth/
      donor/
      recipient/
      admin/
      donations/
      redemptions/
      population-types/
    components/          # shared UI only
    layouts/
    lib/
    test/
  e2e/
  public/
  .env.example
  README.md
  BACKEND_GAPS.md
```

דרישות הנדסיות:

- Types מדויקים לכל request/response; אין `any`.
- API client מרכזי עם timeout, auth header ו־error normalization.
- query keys עקביים ו־invalidation מדויק לאחר mutations.
- route-level lazy loading למסכים כבדים.
- Error Boundary ברמת האפליקציה וברמת אזורים משמעותיים.
- אין business logic בתוך components תצוגתיים.
- אין קריאות API ישירות מתוך components; השתמש ב־service/query hooks.
- אין תלות של feature אחד בפרטים פנימיים של feature אחר.
- feature flags ממקור מרכזי typed.
- environment validation בזמן startup עבור משתנים חיוניים.
- אין מספרי אוכלוסייה, role strings או status strings “קסומים” מפוזרים בקוד.

## 13. ולידציה וטפסים

- ולידציית הלקוח תשקף את DataAnnotations הקיימים, אך השרת נשאר מקור האמת.
- סכומים: לכל היותר שתי ספרות אחרי הנקודה, תחום תקין, ללא scientific notation.
- ילדים: מספר שלם לא שלילי.
- סיסמה: לפחות 8 תווים; הוסף confirm password בצד הלקוח אף שאינו נשלח לשרת.
- טלפון: קלט נוח למספר ישראלי, אך אל תנרמל בצורה שעלולה לשנות משמעות בלי בדיקות.
- תאריך תפוגה: חייב להיות בעתיד; שלח ISO מלא והבהר timezone.
- תיאור וסיבת בקשה: counter עד 250 תווים.
- מנע double submit בכל טופס.
- שמור server validation errors ליד השדה כאשר ניתן למפות אותם.

## 14. בדיקות וקריטריוני איכות

כתוב לפחות:

### Unit/component tests

- error normalizer לכל envelope ידוע.
- JWT decoding ו־role extraction, כולל token פג.
- route guards לכל role.
- formatters לכסף, תאריך וסטטוס.
- validation schemas לטפסים המרכזיים.
- approve/reject dialogs והתנהגות ב־409.
- session cleanup ב־401.

### Integration tests עם MSW

- login מוצלח וכושל.
- רישום תורם ונתרם.
- תורם יוצר, מעדכן ומבטל תרומה.
- נתרם מעדכן פרופיל ורואה זכאות חדשה מהשרת.
- מנהל מסנן, מדפדף, מאשר ודוחה מימוש.
- טיפול ב־Problem Details וב־ValidationProblemDetails.

### Playwright E2E

- מסלול תורם קריטי.
- מסלול נתרם קריטי.
- מסלול מנהל: login → pending redemptions → detail → approve/reject.
- route protection וניסיון כניסה עם role שגוי.
- viewport מובייל אחד ודסקטופ אחד לפחות.

הרץ לפני מסירה:

- install נקי לפי lockfile
- lint
- typecheck
- unit/integration tests
- production build
- E2E קריטי

תקן failures שנגרמו מהעבודה. אל תטען שהבדיקות עברו אם לא הורצו בפועל.

## 15. קריטריוני קבלה

המשימה נחשבת גמורה רק כאשר:

1. ניתן להריץ את ה־Frontend בהוראות קצרות וברורות.
2. login מנתב לפי role ומגן על routes.
3. כל הזרימות הנתמכות כיום לתורם, נתרם ומנהל עובדות מול ה־API האמיתי.
4. אין קריאות production ל־endpoints שלא קיימים.
5. חסימת population types מטופלת באופן שקוף ולא באמצעות IDs קשיחים.
6. פעולות approve/reject בטוחות מפני double submit ומטפלות ב־409.
7. כל הטפסים מציגים validation ושגיאות שרת בצורה ברורה.
8. הממשק RTL, רספונסיבי ונגיש.
9. אין `any`, secrets, console logs רגישים או warnings משמעותיים.
10. production build, typecheck והבדיקות הקריטיות עוברים.
11. קיימים `README.md`, `.env.example` ו־`BACKEND_GAPS.md` מעודכנים.
12. נמסר סיכום קבצים, החלטות, פערים, בדיקות שהורצו ומה עדיין דורש Backend.

## 16. סדר עבודה מחייב

1. בדוק repository, branch, commit, `AGENTS.md` ושינויים מקומיים.
2. קרא Controllers, DTOs, Swagger/tests ועדכן לעצמך API inventory.
3. הפק `BACKEND_GAPS.md` ראשוני.
4. אתחל את פרויקט ה־Frontend והגדר lint, typecheck, tests, RTL ו־theme.
5. בנה API client, contracts, error normalization ו־authentication.
6. ממש routes/layouts ו־guards.
7. ממש את האזור הציבורי.
8. ממש תורם.
9. ממש נתרם.
10. ממש מנהל עבור redemptions הקיימים.
11. הוסף feature flags ומצב mock אופציונלי ליכולות החסרות.
12. הרץ בדיקות, בצע QA רספונסיבי ונגישות בסיסית ותקן בעיות.
13. עדכן תיעוד ומסור סיכום עובדתי.

אם מתגלה חוסר קטן שניתן לעקוף בבטחה בצד ה־Frontend, בצע את העקיפה ותעד אותה. אם החוסר משנה חוזה מוצר, אבטחה, הרשאות או לוגיקה עסקית, אל תנחש—עצור רק את אותה יכולת, המשך בשאר העבודה, ותעד החלטה נדרשת.

## 17. פורמט הדיווח הסופי

בסיום החזר:

- מה נבנה בפועל.
- נתיבי המסכים המרכזיים.
- החלטות ארכיטקטורה חשובות.
- endpoints קיימים שחוברו.
- פערי Backend שמונעים השלמה מלאה.
- בדיקות ופקודות שהורצו, עם תוצאה אמיתית.
- קבצים מרכזיים שנוספו או שונו.
- הוראות הרצה מקומיות.
- צעדים מומלצים להמשך, לפי עדיפות.

אל תמסור רק הסבר. השאר repository במצב שבו מפתח אחר יכול להתקין, להריץ, לבדוק ולהמשיך לעבוד מיד.
