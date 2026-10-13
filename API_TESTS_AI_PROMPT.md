# משימה למודל AI: יצירת מערך בדיקות API למערכת DonationMatchingSystem

## התפקיד שלך

אתה מהנדס בדיקות בכיר ומפתח ASP.NET Core. עבוד ישירות בתוך repository זה וייצר מערך בדיקות אוטומטי, אמין וניתן להרצה עבור ה־Backend. עיקר הדגש הוא בדיקות API מקצה לקצה שמוודאות לא רק מה מוחזר ב־HTTP, אלא גם שכל שינוי המצב הנדרש אכן התרחש — או לא התרחש — במסד הנתונים, בתור האירועים ובתהליך ה־matching.

אל תסתפק בתוכנית בדיקות או בדוגמאות קוד. צור בפועל את פרויקט הבדיקות, fixtures, helpers והבדיקות, הרץ אותן, ותקן את קוד הבדיקות עד שהן מתקמפלות ורצות. אל תשנה לוגיקה עסקית רק כדי לגרום לבדיקה לעבור.

## כללי עבודה מחייבים

- התחל בסריקה עצמאית של כל ה־repository. אל תניח שהסיכום במסמך זה מחליף קריאה בקוד.
- קרא במיוחד את:
  - `src/BackendService/Program.cs`
  - `src/BackendService/Controllers/`
  - `src/BackendService/Services/`
  - `src/BackendService/Repos/`
  - `src/BackendService/Data/`
  - `src/BackendService/Models/`
  - `src/BackendService/DTOs/`
  - `src/BackendService/ExceptionHandling/`
  - `src/BaseScriptService/main.py`
  - `docker-compose.yml`
  - `docs/specification/donation-system-v3.md`
- בדוק `git status` לפני כל שינוי. שמור כל שינוי קיים של המשתמש ואל תדרוס קבצים שאינם שייכים למשימת הבדיקות. נכון למועד הכנת מסמך זה, `src/BaseScriptService/main.py` כבר מכיל שינוי מקומי של המשתמש.
- אל תשתמש במסד הפיתוח ואל תמחק ממנו נתונים. בדיקות אינטגרציה חייבות לעבוד מול MySQL מבודד וזמני.
- אין להריץ migrations או לשנות את סכמת ה־production. מותר ליצור schema רק בתוך מסד זמני של הבדיקות.
- אין לבצע assertions על פרטים לא יציבים כגון מזהי auto-increment קבועים, סדר רשימה שלא הוגדר, timestamps מדויקים או `traceId` קבוע.
- אין להשתמש ב־`Thread.Sleep` או השהיות קשיחות כדי להמתין ל־background worker. השתמש ב־eventual assertion עם timeout קצר ו־polling.
- כל בדיקה צריכה להיות עצמאית, דטרמיניסטית, ניתנת להרצה במקביל או מסומנת במפורש כלא־מקבילית כאשר יש סיבה מוצדקת.
- השתמש בתבנית Arrange / Act / Assert ובשמות בדיקות המתארים תנאי, פעולה ותוצאה.
- אל תחשוף סיסמאות, JWT signing key, connection strings או hashes בפלט הבדיקות.

## מצב הפרויקט שנמצא בסריקה הראשונית

- Backend יחיד ב־ASP.NET Core, target framework `net10.0`.
- EF Core עם Pomelo מול MySQL.
- JWT Bearer authentication עם `sub` כמזהה `User.Id` ו־role מסוג `DONOR`, `RECIPIENT` או `ADMIN`.
- שבע ישויות: `User`, `Donor`, `Recipient`, `Admin`, `PopulationType`, `Donation`, `Redemption`.
- אין כרגע solution file ואין פרויקט בדיקות.
- `dotnet build src/BackendService/BackendService.csproj --no-restore` עובר בזמן כתיבת מסמך זה, עם warning בלבד לגבי `Microsoft.Extensions.Hosting`.
- אין migrations. הסכמה מתוארת ב־`src/BaseScriptService/main.py`.
- קיימים raw SQL ו־`SELECT ... FOR UPDATE`, ולכן EF InMemory או SQLite אינם תחליף נאמן לבדיקות האינטגרציה של ה־DB.
- `MatchingEventQueue` הוא bounded channel בזיכרון, ו־`DonationMatchingWorker` הוא hosted background service.
- מסמך האפיון מתאר יעד רחב יותר תחת `/api/v1`, אבל הוא אינו זהה ל־API שממומש כרגע.

## סדר מקורות האמת

הפרד במפורש בין שתי מטרות:

1. **Regression suite למימוש הקיים** — הקוד הפעיל הוא מקור האמת לנתיבים שקיימים ולמבני התגובה הקיימים. בדיקות אלה צריכות לעבור לאחר השלמת העבודה.
2. **Specification gaps** — `docs/specification/donation-system-v3.md` הוא מקור לדרישות מתוכננות ולזיהוי פערים. אל תכתוב בדיקות רגילות שנכשלות רק משום שנתיב עתידי טרם מומש. תעד פערים ב־`tests/TEST_GAPS.md`. אם יש ערך בבדיקת דרישה עתידית, צור אותה כבדיקה מדולגת עם סיבה ברורה ו־Trait/Category בשם `SpecificationGap`.

כאשר הקוד, הסכמה והאפיון סותרים זה את זה:

- אל תנחש ואל תשנה production behavior.
- כתוב בדיקת characterization אם ההתנהגות הקיימת בטוחה וברורה.
- אם ההתנהגות נראית כבאג אבטחה, עקביות, הרשאה או כספים — תעד אותה כפער בסיכון גבוה ואל תהפוך אותה לחוזה רצוי.

## ה־API הממומש שיש לבדוק

### Authentication

- `POST /api/Auth/register/donor`
- `POST /api/Auth/register/recipient`
- `POST /api/Auth/login`

שים לב: `[Route("api/[controller]")]` יוצר בפועל `api/Auth`; אל תחליף אותו בנתיבי `/api/v1` מהאפיון.

### Donations — כל ה־controller מוגן בתפקיד DONOR

- `POST /api/donations`
- `GET /api/donations/show-donations`
- `PUT /api/donations/{id:int}`
- `POST /api/donations/{id:int}/cancel`

### Authorization probe endpoints

- `GET /api/test/donor`
- `GET /api/test/admin`
- `GET /api/test/donor-or-admin`

גם אם אלה endpoints זמניים, בדוק אותם כל עוד הם קיימים, משום שהם מאמתים את מיפוי claims והתפקידים.

## תשתית הבדיקות המבוקשת

צור פרויקט בדיקות, מומלץ בשם `tests/BackendService.ApiTests`, עם xUnit ו־`Microsoft.AspNetCore.Mvc.Testing`. השתמש בגרסאות packages התואמות ל־.NET 10 ולחבילות הקיימות בפרויקט.

לבדיקות אינטגרציה:

- העדף Testcontainers for MySQL עם image התואם ל־MySQL 8.4 שב־`docker-compose.yml`.
- ה־container צריך להיות משותף באופן יעיל ל־test collection, אך הנתונים צריכים להתאפס באופן בטוח בין בדיקות.
- אל תתחבר אוטומטית ל־`localhost:3310` ואל תשתמש ב־`donationDb` של המפתח.
- צור את שבע הטבלאות במסד הזמני בלבד. אפשר להשתמש ב־`EnsureCreatedAsync` רק ב־test infrastructure, או להריץ schema test fixture המבוסס על הסכמה הקיימת. אין להוסיף migration לפרויקט ה־Backend.
- ודא ש־MySQL מוכן לפני יצירת ה־`WebApplicationFactory` וש־`ServerVersion.AutoDetect` מקבל connection string של ה־container.
- נטרל או השאר ריק את `ADMIN_SEED` בזמן startup, כדי שהאפליקציה לא תשאל טבלה לפני יצירת ה־schema. צור Admin test user באופן מפורש ב־fixture כשצריך.
- ספק ערכי JWT ייעודיים לבדיקות דרך configuration override; אל תשתמש בסוד שב־`appsettings.json`.
- אם `WebApplicationFactory<Program>` דורש חשיפה של top-level `Program`, מותר לבצע שינוי testability מינימלי, למשל `public partial class Program { }`. אל תשנה התנהגות עסקית.
- הגדר client כך שניתן יהיה להבחין במדויק בין 401, 403 ו־redirect. השתמש ב־HTTPS base address או טפל במפורש ב־HTTPS redirection.
- בנה helpers ליצירת משתמשים ופרופילים, התחברות, הוספת Bearer token, seed של PopulationType וקריאת JSON.
- אל תעשה mock ל־HTTP pipeline בבדיקות API. בקשה צריכה לעבור routing, model binding, validation, authentication, authorization, controller, service, repository ו־MySQL.
- עבור כללי matching שקשה להפעיל דרך endpoint בלבד, הוסף בדיקות אינטגרציה ברמת service עם MySQL האמיתי. אפשר להוסיף unit tests ממוקדים לחישובים טהורים, אך הם אינם מחליפים בדיקות API.

אם Docker אינו זמין בסביבת ההרצה, אל תעבור בשקט ל־InMemory. השאר הודעת skip/diagnostic ברורה ותעד ב־README של הבדיקות כיצד להריץ עם Docker או עם MySQL זמני מפורש.

## מטריצת בדיקות חובה

### 1. רישום תורם

בדוק לפחות:

- payload תקין מחזיר 201, `message` נכון ו־`userId` חיובי.
- נוצר `User` פעיל עם `UserType = DONOR` ונוצר `Donor` המקושר אליו.
- הסיסמה נשמרת כ־hash תקין ואינה נשמרת או מוחזרת כטקסט גלוי.
- `CreatedAt` נקבע בטווח זמן סביר סביב הבקשה.
- לא נוצרו `Recipient` או `Admin` עבור אותו user.
- טלפון כפול מחזיר 409 במבנה `application/problem+json`, ולא נוצרת רשומה נוספת.
- name חסר/ריק, phone חסר או לא תקין, password חסר או קצר מ־8, name/phone ארוכים מדי ו־JSON לא תקין מחזירים 400 ואינם משנים DB.
- שדות JSON עודפים אינם מאפשרים ללקוח לקבוע `UserType`, `IsActive`, `Id` או `PasswordHash`.
- במידת האפשר, בדוק ששמירת User ו־Donor אטומית ושאין User יתום במקרה של כשל בשמירה.

### 2. רישום נתרם

בדוק לפחות:

- payload תקין מול `PopulationType` פעיל מחזיר 201 ונוצרים `User` מסוג `RECIPIENT` ו־`Recipient` מקושר.
- `EligibilityAmount` ו־`RemainingEligibility` מחושבים לפי המימוש הנוכחי: `max(0, baseAmount + children * 500 - income * 0.20)`, עיגול לשתי ספרות ב־`AwayFromZero`.
- נשמרים `PopulationTypeId`, `IncomeAmount`, `AmountOfChildren` ו־`RequestReason` הנכונים.
- population type חסר או לא פעיל מחזיר 400 ולא יוצר User יתום.
- income שלילי, יותר משתי ספרות אחרי הנקודה, children שלילי, reason ארוך מ־250 ושאר שגיאות DataAnnotations מחזירים 400 ללא side effects.
- טלפון כפול מחזיר 409 ללא רשומות נוספות.
- לאחר save מוצלח נשלח אירוע `RecipientRegistered`; אל תצרוך בטעות את האירוע אם באותה בדיקה worker אמור לעבד אותו.
- כאשר קיימת תרומה מתאימה, בדוק באופן eventual שה־worker יוצר `Redemption` במצב `PENDING` עם הסכום הצפוי.
- כאשר אין התאמה בסוג האוכלוסייה, החשבון לא פעיל, הזכאות אפס או התרומה לא זמינה — לא נוצר redemption.

### 3. Login ו־JWT

בדוק לפחות:

- credentials תקינים לכל תפקיד רלוונטי מחזירים 200 עם `accessToken` לא ריק ו־`tokenType = Bearer`.
- פענח את ה־JWT ובדוק signature, issuer, audience, lifetime, `sub`, role ו־`jti`; אל תסתפק בכך שהמחרוזת נראית כמו token.
- `sub` הוא `User.Id`, לא `Donor.Id`/`Recipient.Id`/`Admin.Id`.
- סיסמה שגויה, טלפון לא קיים ו־User לא פעיל מחזירים 401 עם הודעה בטוחה ואינם מסגירים איזה חלק נכשל.
- קלט חסר מחזיר 400.
- אין hash, סיסמה או secret בתגובה.
- שני logins מוצלחים יוצרים `jti` שונים.

### 4. Authentication ו־Authorization

לכל endpoint מוגן רלוונטי בדוק:

- ללא token: 401.
- token פג תוקף, חתימה שגויה, issuer שגוי או audience שגוי: 401.
- token תקין עם role שאינו מורשה: 403.
- token חתום ותקין אך ללא `sub`, או עם `sub` שאינו integer, מגיע ל־controller במקום שבו role מתאים ומחזיר 401.
- `GET /api/test/donor` מחזיר לתורם message, `userId` ו־role נכונים.
- `GET /api/test/admin` פתוח רק למנהל.
- `GET /api/test/donor-or-admin` פתוח לתורם ולמנהל וחסום לנתרם.
- אין גישה לנתוני תורם אחר באמצעות שינוי id או payload.

### 5. יצירת תרומה

בדוק לפחות:

- תורם פעיל עם payload תקין מקבל 201, message ו־`donationId` חיובי.
- DB מכיל donation עם `DonorId` שנגזר מה־JWT, סוג אוכלוסייה נכון, `TotalAmount`, `RemainingAmount = TotalAmount`, תקרה, תיאור, `Status = ACTIVE`, `CreatedAt` ו־`ExpirationDate` ב־UTC.
- שדה `DonorId` עודף ב־JSON אינו מאפשר התחזות לתורם אחר.
- population type חסר/לא פעיל, סכומים אפס/שליליים/מחוץ לטווח ותאריך שאינו בעתיד מחזירים 400 ללא donation וללא event.
- תיאור מעל 250 מחזיר 400.
- תורם לא פעיל או user/profile לא עקביים מקבלים את קוד השגיאה שה־service וה־global handler מגדירים, ללא יצירת donation.
- לאחר save מוצלח נשלח `DonationCreated` ורק לאחר save.
- כאשר קיימים recipients מתאימים, בדוק eventual creation של redemptions; כאשר אינם מתאימים, ודא שלא נוצרו.

### 6. קבלת התרומות של התורם

בדוק לפחות:

- 200 ומערך ריק לתורם ללא תרומות.
- מוחזרות רק התרומות השייכות לתורם המזוהה, ולא תרומות של תורם אחר.
- לכל פריט מוחזרים בדיוק השדות המותרים ב־`DonationResponseDto`, עם ערכים נכונים.
- אין entity navigation properties, פרטי משתמש, password hash או פרטי recipients בתגובה.
- profile תורם חסר מחזיר 404 Problem Details.
- בדוק ותעד את ההתנהגות בפועל עבור תורם שהושבת אחרי הנפקת token. האפיון דורש אכיפת `IsActive` בכל פעולה רגישה; אם GET עדיין מצליח, רשום זאת כפער אבטחה ואל תגדיר זאת כחוזה רצוי.

### 7. עדכון תרומה

בדוק לפחות:

- עדכון תקין בידי הבעלים מחזיר 204 ומשנה את השדות ב־DB.
- אם כבר מומש סכום, שינוי `TotalAmount` משמר את הסכום שמומש ומחשב `RemainingAmount = newTotal - redeemedAmount`.
- אין שינוי של `DonorId`, `PopulationTypeId`, `CreatedAt` או `Status`.
- תרומה לא קיימת: 404; תרומה של תורם אחר: 403.
- תרומה לא ACTIVE או שפגה: 409.
- תאריך עבר, סכומים לא חיוביים ו־DataAnnotations לא תקינים: 400.
- `TotalAmount` חדש הנמוך מהסכום שכבר מומש: 409 וללא שינוי חלקי.
- כל כשל משאיר את כל שדות התרומה ללא שינוי והטרנזקציה נסגרת/מתבטלת כראוי.
- בדוק את צורת השגיאה בפועל. שים לב שה־controller עדיין מכיל catch לחריגים ישנים, בעוד ה־service משתמש בחלק מהמקרים בחריגים custom שמטופלים ב־`GlobalExceptionHandler`.

### 8. ביטול תרומה

בדוק לפחות:

- בעלים של תרומה ACTIVE ולא פגה מקבל 204.
- `Donation.Status` משתנה ל־`CANCELLED`.
- כל redemption מסוג `PENDING` של התרומה משתנה ל־`CANCELLED`.
- redemptions מסוג `COMPLETED`, `REJECTED` או שכבר `CANCELLED` אינם משתנים.
- תרומה לא קיימת: 404; תורם אחר או תורם לא פעיל: 403; תרומה לא פעילה או שפגה: 409.
- כשל באמצע אינו משאיר donation מבוטלת עם pending redemptions שלא בוטלו, או להפך.
- ביטול חוזר אינו מצליח כאילו היה שינוי חדש; אמת את 409 הקיים.
- לאחר ביטול לא נוצרים matches חדשים עבור התרומה.
- בדוק בנפרד את מבני התגובה הקיימים: חלק משגיאות הביטול מוחזרות כרגע ישירות מה־controller ולא כ־Problem Details.

### 9. Matching ו־side effects אסינכרוניים

הוסף בדיקות אינטגרציה ממוקדות עבור `DonationMatchingService` וה־worker:

- סכום התאמה הוא המינימום בין `Donation.RemainingAmount`, `Recipient.RemainingEligibility` והיתרה מתוך `MaxAmountPerRecipient` לאחר סכומי `COMPLETED` קודמים.
- אין סכום שלילי; אם התקרה כבר נוצלה, לא נוצר match.
- נדרש אותו `PopulationTypeId`.
- donor ו־recipient חייבים להיות פעילים.
- donation חייבת להיות ACTIVE, לא פגה ובעלת יתרה חיובית.
- recipient חייב להיות בעל יתרת זכאות חיובית.
- `PENDING` או `REJECTED` קיים לאותו זוג donation/recipient חוסם יצירה נוספת לפי המימוש הנוכחי.
- בדוק במפורש מה קורה עם `CANCELLED` ו־`COMPLETED` קיימים ותעד אם ההתנהגות אינה תואמת לדרישה העסקית.
- `DonationCreated` מפעיל התאמה לכל recipients מתאימים; `RecipientRegistered` מפעיל התאמה מול donations זמינות.
- אירוע בודד לא יוצר duplicate redemption לאותו זוג.
- בדוק תרחיש concurrency אמיתי מול MySQL כדי לוודא ששתי פעולות matching מקבילות לא יוצרות duplicate PENDING לאותו זוג.
- בדוק rollback כאשר שמירת redemption נכשלת.
- worker לא קורס לצמיתות בעקבות אירוע כושל ויכול לעבד אירוע אחריו.
- אל תניח שיצירת `PENDING` מפחיתה יתרות — אמת את ההתנהגות הקיימת ותעד את סיכון ה־overbooking אם סכומי pending מצטברים יכולים לעבור את יתרת התרומה.

### 10. חוזי שגיאה ו־HTTP

בדוק לפחות:

- `GlobalExceptionHandler` מחזיר `application/problem+json` עם `status`, `title`, `detail`, `instance` ו־`traceId` עבור 400/403/404/409/500.
- שגיאה בלתי צפויה מחזירה detail כללי ולא stack trace, exception type, SQL או connection string.
- model validation מחזיר ValidationProblemDetails וכולל errors לשדות הרלוונטיים.
- 401/403 של middleware אינם מוחלפים בטעות ב־404 או 500.
- JSON response משתמש ב־camelCase בהתאם לברירת המחדל בפועל.
- 204 אינו מכיל body.
- תעד ב־`TEST_GAPS.md` שהאפיון מבקש גם business `code` ו־`correlationId`, בעוד המימוש הנוכחי מוסיף `traceId` ואינו מוסיף `code`.

### 11. מקרי קצה, גבולות ואבטחה

- ערכי decimal בדיוק בגבולות ומעט מעבר לגבולות.
- rounding לשתי ספרות וסכומים שעלולים לחשוף שגיאת floating point; השתמש תמיד ב־`decimal`.
- תאריכי UTC, offset חיובי/שלילי, ערך בדיוק "עכשיו" ותאריך רחוק בעתיד.
- Unicode בשם ובתיאור, מחרוזת ריקה, whitespace בלבד ואורכי 50/250 בדיוק ומעליהם.
- JSON עם סוגי נתונים שגויים, null ושדות בלתי מוכרים.
- ניסיון mass assignment לשדות server-owned.
- ניסיון SQL injection דרך phone/name/description צריך להישמר כטקסט או להידחות בלי לפגוע בסכמה.
- תגובות לעולם אינן כוללות `PasswordHash`.
- בדוק רישום מקבילי של אותו phone. הדרישה הרצויה היא לכל היותר משתמש אחד ותגובה נשלטת, לא 500; אם המימוש מחזיר 500 עקב race בין בדיקה ל־unique constraint, תעד כפער בסיכון גבוה.

## בדיקות unit משלימות

בנוסף לבדיקות API, הוסף unit tests קצרים וממוקדים עבור:

- `EligibilityService.CalculateEligibility`: נוסחה, רצפה באפס, rounding, וקלט שלילי.
- `JwtTokenService.GenerateToken`: claims וקונפיגורציה חסרה/expiration לא חיובי.
- `DonationMatchingService.CalculateMatchingAmountAsync`: גבולות של יתרות ותקרה מצטברת.

אל תכפיל בבדיקות unit את כל מה שכבר נבדק דרך API.

## פערים ידועים שחשוב לא להתעלם מהם

במהלך העבודה אמת בעצמך את הנקודות הבאות ותעד אותן אם הן עדיין נכונות:

- האפיון מציע נתיבי `/api/v1` רבים שאינם קיימים במימוש הנוכחי.
- האפיון אומר שאלגוריתם הזכאות עדיין דורש אישור, אך הקוד כבר מממש נוסחה קבועה.
- האפיון דורש `MaxAmountPerRecipient <= TotalAmount`; ה־DTO/service הנוכחיים אינם נראים כאוכפים זאת.
- האפיון דורש אכיפת user activity בכל פעולה רגישה; קריאת רשימת תרומות עשויה לא לבדוק `Donor.User.IsActive`.
- יצירת PENDING redemption אינה מפחיתה או שומרת יתרה, ולכן כמה matches עשויים לייצר התחייבות מצטברת גבוהה מהיתרה.
- `HasBlockedMatchAsync` חוסם גם `REJECTED`, התנהגות שדורשת אימות עסקי.
- מבני שגיאה אינם אחידים בין global handler, model validation, login, update/cancel ו־authentication middleware.
- אין endpoint עסקי להשלמת redemption ולעדכון שתי היתרות, אף שהוא מופיע באפיון.
- `DonationMatchingWorker` בולע שגיאת אירוע לאחר logging ואין מנגנון retry/dead-letter נראה לעין.

אל תתקן נקודות אלה במסגרת משימת הבדיקות, אלא אם שינוי זעיר נדרש אך ורק כדי לאפשר testability. כתוב אותן ב־`tests/TEST_GAPS.md` עם severity, evidence, test coverage והמלצה.

## מבנה תוצרים מבוקש

צור לפחות:

- פרויקט test תחת `tests/BackendService.ApiTests/`.
- API integration tests לפי קבוצות ה־endpoints.
- MySQL/Testcontainers fixture ו־`WebApplicationFactory` מותאם.
- builders/helpers ל־seed, JWT ו־HTTP בלי להסתיר assertions חשובים.
- unit tests ממוקדים לשירותים הטהורים והחישובים.
- `tests/README.md` עם prerequisites ופקודות הרצה.
- `tests/TEST_GAPS.md` עם פערי מפרט/מימוש וסיכונים שהתגלו.
- solution file בשורש או עדכון solution קיים, אם הדבר מקל על `dotnet test`, בלי לשנות את מבנה ה־production שלא לצורך.

## איכות assertions

בכל happy path של פעולה שמשנה מצב, בצע שלוש שכבות אימות:

1. **HTTP** — status, content type, headers ומבנה JSON.
2. **Persistence** — הרשומות והשדות הנכונים במסד לאחר שימוש ב־DbContext חדש, כדי לא לקרוא state cached.
3. **Side effects** — event, background matching, שינוי רשומות קשורות או היעדר side effect.

בכל failure path, אמת גם:

- status ומבנה שגיאה.
- שאין שינוי חלקי ב־DB.
- שלא נשלח event ולא נוצר match כאשר הפעולה הראשית נכשלה.

העדף assertions מדויקים. אל תסתפק ב־`IsSuccessStatusCode`, `NotNull` או ספירת רשומות בלבד כאשר ניתן לאמת את התוכן והקשרים.

## הרצה וסיום

בסוף העבודה:

- הרץ `dotnet build` על כל ה־solution/פרויקטים.
- הרץ את כל הבדיקות לפחות פעם אחת מתחילתן על DB זמני נקי.
- הרץ שוב כדי לאתר תלות בסדר, זליגת state או flakiness.
- אם אפשר, הרץ גם קבוצה רלוונטית במקביל.
- אל תציג הצלחה אם בדיקות לא הורצו בפועל.
- דווח אילו קבצים נוצרו/שונו, מספר הבדיקות שעברו/נכשלו/דולגו, זמן הרצה, וכל מגבלה סביב Docker.
- אם בדיקה חושפת באג production, אל תרכך את ה־assertion ואל תשנה את ה־production כדי להסתירו. סווג אותה נכון כ־regression blocker או specification gap והסבר בקצרה.

התוצאה הרצויה היא test suite שניתן להריץ מקומית וב־CI, שמגן על חוזה ה־HTTP, הרשאות, התמדה, טרנזקציות וה־matching האסינכרוני של המערכת.
