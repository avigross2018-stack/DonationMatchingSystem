<table>
<colgroup>
<col style="width: 100%" />
</colgroup>
<tbody>
<tr>
<td><p><strong><span dir="rtl">מסמך אפיון</span></strong><span dir="ltr"></span></p>
<p><strong><span dir="rtl">אפיון מערכת סימולציית תרומות ונתרמים</span></strong><span dir="ltr"></span></p>
<p><span dir="rtl">מסמך דרישות עסקיות טכניות וארכיטקטוניות</span><span dir="ltr"></span></p></td>
</tr>
</tbody>
</table>

<span dir="ltr"></span>

**<span dir="rtl">פרטי המסמך</span>**<span dir="ltr"></span>

| **<span dir="rtl">גרסה</span>**<span dir="ltr"></span> | <span dir="ltr"></span><span dir="rtl">3.0 עדכון מבנה משתמשים</span><span dir="ltr"></span> |
|----|----|
| **<span dir="rtl">תאריך</span>**<span dir="ltr"></span> | <span dir="ltr"></span><span dir="rtl">11 באוקטובר 2026</span><span dir="ltr"></span> |
| **<span dir="rtl">סטטוס</span>**<span dir="ltr"></span> | <span dir="rtl">אפיון מעודכן בכפוף להכרעות העסקיות ולבדיקת הסכמה</span><span dir="ltr"></span> |
| **<span dir="rtl">טכנולוגיית</span> <span dir="ltr">Backend</span>**<span dir="ltr"></span> | ASP.NET Core Web API (C#) |
| **<span dir="rtl">בסיס נתונים</span>**<span dir="ltr"></span> | <span dir="ltr">MySQL</span> <span dir="rtl">קיים שנוצר על ידי סקריפט</span> <span dir="ltr">Python</span> <span dir="rtl">חיצוני</span><span dir="ltr"></span> |

<span dir="ltr"></span>

|  |
|----|
| **<span dir="rtl">מטרת המסמך</span>**<span dir="ltr"></span> <span dir="rtl">היא לקבע אפיון ישים לפיתוח ה־</span><span dir="ltr">API</span><span dir="rtl">, להבחין בין החלטות סופיות לבין פרטים שטרם הוגדרו, ולהבטיח שהמימוש לא ייצור ולא ישנה את בסיס הנתונים הקיים.</span><span dir="ltr"></span> |

**<span dir="rtl">תוכן עניינים</span>**

<span dir="rtl">תקציר מנהלים</span>

<span dir="ltr"></span><span dir="rtl">1 מטרות היקף ועקרונות</span>

> <span dir="ltr"></span><span dir="rtl">1.1 מטרות המערכת</span>
>
> <span dir="ltr"></span><span dir="rtl">1.2 היקף כלול</span>
>
> <span dir="ltr"></span><span dir="rtl">1.3 מחוץ להיקף</span>
>
> <span dir="ltr"></span><span dir="rtl">1.4 עקרונות מנחים</span>

<span dir="ltr"></span><span dir="rtl">2 החלטות סופיות ופערים</span>

<span dir="ltr"></span><span dir="rtl">3 בעלי תפקידים והרשאות עסקיות</span>

<span dir="ltr"></span><span dir="rtl">4 מודל הנתונים ושבע הטבלאות</span>

> <span dir="ltr">4.1 User</span> <span dir="rtl">משתמשים</span>
>
> <span dir="ltr">4.2 Donor</span> <span dir="rtl">תורמים</span>
>
> <span dir="ltr">4.3 Recipient</span> <span dir="rtl">נתרמים</span>
>
> <span dir="ltr">4.4 Admin</span> <span dir="rtl">מנהלים</span>
>
> <span dir="ltr">4.5 PopulationType</span> <span dir="rtl">סוגי אוכלוסייה</span>
>
> <span dir="ltr">4.6 Donation</span> <span dir="rtl">תרומות</span>
>
> <span dir="ltr">4.7 Redemption</span> <span dir="rtl">מימושים</span>
>
> <span dir="ltr"></span><span dir="rtl">4.8 קשרים וקרדינליות</span>
>
> <span dir="ltr"></span><span dir="rtl">4.9 כללי עקביות בין טבלאות</span>

<span dir="ltr"></span><span dir="rtl">5 סטטוסים ומעברי מצב</span>

> <span dir="ltr"></span><span dir="rtl">5.1 סטטוס חשבון</span>
>
> <span dir="ltr"></span><span dir="rtl">5.2 סטטוס תרומה</span>
>
> <span dir="ltr"></span><span dir="rtl">5.3 מצב זכאות וזמינות מחושבים</span>
>
> <span dir="ltr"></span><span dir="rtl">5.4 סטטוס מימוש</span>

<span dir="ltr"></span><span dir="rtl">6 תהליכים עסקיים</span>

> <span dir="ltr"></span><span dir="rtl">6.1 רישום והתחברות</span>
>
> <span dir="ltr"></span><span dir="rtl">6.2 עדכון נתוני סיוע וזכאות</span>
>
> <span dir="ltr"></span><span dir="rtl">6.3 יצירת תרומה</span>
>
> <span dir="ltr"></span><span dir="rtl">6.4 התאמת תרומות לנתרם</span>
>
> <span dir="ltr"></span><span dir="rtl">6.5 מימוש</span>
>
> <span dir="ltr"></span><span dir="rtl">6.6 ביטול תפוגה וסגירה</span>

<span dir="ltr"></span><span dir="rtl">7 חוקי זכאות תעדוף ומימוש</span>

> <span dir="ltr"></span><span dir="rtl">7.1 תנאי סף לזכאות</span>
>
> <span dir="ltr"></span><span dir="rtl">7.2 נתוני אלגוריתם הזכאות</span>
>
> <span dir="ltr"></span><span dir="rtl">7.3 כללי תעדוף</span>
>
> <span dir="ltr"></span><span dir="rtl">7.4 חוקי מימוש</span>

<span dir="ltr"></span><span dir="rtl">8 ולידציות וכללי עקביות</span>

> <span dir="ltr"></span><span dir="rtl">8.1 קודי שגיאה והתנהגות</span>

<span dir="ltr"></span><span dir="rtl">9 אבטחה והרשאות</span>

<span dir="ltr"></span><span dir="rtl">10 ממשק</span> <span dir="ltr">REST API</span>

> <span dir="ltr"></span><span dir="rtl">10.1 כללי חוזה</span>
>
> <span dir="ltr"></span><span dir="rtl">10.2 אימות וחשבון</span>
>
> <span dir="ltr"></span><span dir="rtl">10.3 תורמים ותרומות</span>
>
> <span dir="ltr"></span><span dir="rtl">10.4 נתרמים וזכאות</span>
>
> <span dir="ltr"></span><span dir="rtl">10.5 מימושים וסוגי אוכלוסייה</span>
>
> <span dir="ltr"></span><span dir="rtl">10.6 מנהלים ובקרה</span>
>
> <span dir="ltr"></span><span dir="rtl">10.7 מבנה שגיאה</span>

<span dir="ltr"></span><span dir="rtl">11 ארכיטקטורת המערכת</span>

> <span dir="ltr"></span><span dir="rtl">11.1 מבנה לוגי סופי</span>
>
> <span dir="ltr">11.2 Repositories</span> <span dir="rtl">נדרשים</span>
>
> <span dir="ltr"></span><span dir="rtl">11.3 מיפוי</span> <span dir="ltr">EF Core</span> <span dir="rtl">ללא שינוי</span> <span dir="ltr">DB</span>
>
> <span dir="ltr"></span><span dir="rtl">11.4 רכיבים רוחביים</span>

<span dir="ltr"></span><span dir="rtl">12 דרישות לא פונקציונליות</span>

<span dir="ltr"></span><span dir="rtl">13 דיווח בקרה ותיעוד</span>

<span dir="ltr"></span><span dir="rtl">14 קריטריוני קבלה</span>

<span dir="ltr"></span><span dir="rtl">15 החלטות פתוחות וסיכונים</span>

> <span dir="ltr"></span><span dir="rtl">15.1 סיכונים מרכזיים ומענה</span>

<span dir="ltr"></span><span dir="rtl">16 מילון מונחים</span>

<span dir="rtl">סיכום מחייב</span>

# **<span dir="rtl">תקציר מנהלים</span>**

<span dir="rtl">המערכת מנהלת סימולציית תרומות ומימושי סיוע. פרטי הזהות וההתחברות נשמרים פעם אחת בטבלת</span> <span dir="ltr">User</span><span dir="rtl">. טבלאות</span> <span dir="ltr">Donor, Recipient</span> <span dir="rtl">ו־</span><span dir="ltr">Admin</span> <span dir="rtl">מקושרות אליה באמצעות</span> <span dir="ltr">UserId</span> <span dir="rtl">ומכילות רק את הנתונים הייחודיים לתפקיד. תרומה משויכת לסוג אוכלוסייה, ומימוש מקשר ישירות בין התרומה לנתרם. אין במודל טבלת בקשות סיוע או טבלת הקצאות. המערכת אינה מבצעת העברת כסף אמיתי או סליקה.</span>

<span dir="rtl">החלטת הארכיטקטורה הסופית היא יישום</span> <span dir="ltr">ASP.NET Core Web API</span> <span dir="rtl">יחיד, מול</span> <span dir="ltr">Frontend</span> <span dir="rtl">נפרד. זרימת התלות היא</span> <span dir="ltr">Controllers</span> <span dir="rtl">אל</span> <span dir="ltr">Services</span><span dir="rtl">, מהם אל</span> <span dir="ltr">Repository</span> <span dir="rtl">נפרד לכל אחת משבע הטבלאות, משם אל</span> <span dir="ltr">EF Core DbContext</span> <span dir="rtl">ולבסוף אל</span> <span dir="ltr">MySQL</span><span dir="rtl">. בסיס הנתונים כבר נוצר באמצעות סקריפט</span> <span dir="ltr">Python</span> <span dir="rtl">חיצוני, ולכן ה־</span><span dir="ltr">API</span> <span dir="rtl">אינו רשאי ליצור, לשנות, למחוק או לבצע</span> <span dir="ltr">Migration</span> <span dir="rtl">לסכימה.</span>

<span dir="rtl">גרסה זו מתאימה את האפיון לשבע הטבלאות שבמסמך התכנון המקורי:</span> <span dir="ltr">User, Donor, Recipient, Admin, PopulationType, Donation</span> <span dir="rtl">ו־</span><span dir="ltr">Redemption</span><span dir="rtl">. משקלי אלגוריתם הזכאות והנוסחה טרם הוגדרו. יש לאמת את התאמת השמות, הטיפוסים והאילוצים לסכמת</span> <span dir="ltr">MySQL</span> <span dir="rtl">הקיימת בקריאה בלבד; אין לשנות את בסיס הנתונים כדי להתאימו למסמך.</span>

# **<span dir="rtl">1 מטרות היקף ועקרונות</span>**

## **<span dir="rtl">1.1 מטרות המערכת</span>**

- <span dir="rtl">לאפשר לתורם ליצור ולעקוב אחר תרומות בסביבת סימולציה.</span>

- <span dir="rtl">לאפשר לנתרם לעדכן נתוני סיוע בפרופיל, לצפות בזכאותו ובתרומות מתאימות ולבקש מימוש.</span>

- <span dir="rtl">לחשב סכום זכאות עקבי לאחר אישור האלגוריתם והמשקלים; להבחין בין סכום הזכאות לבין היתרה שנותרה למימוש.</span>

- <span dir="rtl">להתאים בין תרומה לנתרם לפי סוג אוכלוסייה, תוקף ויתרות, ולתעד כל מימוש.</span>

- <span dir="rtl">לאפשר למנהל לאשר, לדחות, להשהות ולתעד חריגים תוך שמירת עקיבות מלאה.</span>

- <span dir="rtl">לספק</span> <span dir="ltr">API</span> <span dir="rtl">יציב ל־</span><span dir="ltr">Frontend</span> <span dir="rtl">נפרד בלי להטמיע לוגיקת</span> <span dir="ltr">UI</span> <span dir="rtl">או שינויי סכמת</span> <span dir="ltr">DB</span><span dir="rtl">.</span>

## **<span dir="rtl">1.2 היקף כלול</span>**

- <span dir="rtl">ניהול חשבון</span> <span dir="ltr">User</span> <span dir="rtl">משותף ופרופיל תפקיד נפרד לתורם, לנתרם או למנהל.</span>

- <span dir="rtl">ניהול סוגי אוכלוסייה, תרומות ומימושים לפי שבע הטבלאות המקוריות.</span>

- <span dir="rtl">אימות הרשאות לפי תפקיד ובעלות על רשומה.</span>

- <span dir="rtl">שמירת סכום זכאות ויתרת זכאות בטבלת</span> <span dir="ltr">Recipient</span><span dir="rtl">, ללא הוספת שדות ניקוד או גרסת אלגוריתם.</span>

- <span dir="rtl">מעברי סטטוס מבוקרים וכללי סכום כמות ותוקף.</span>

- <span dir="rtl">חיפוש סינון מיון ודפדוף ברשימות.</span>

- <span dir="rtl">לוגים תפעוליים ומידע הנדרש לביקורת.</span>

## **<span dir="rtl">1.3 מחוץ להיקף</span>**

- <span dir="rtl">יצירה שינוי או מחיקה של טבלאות אינדקסים אילוצים או נתוני</span> <span dir="ltr">Seed</span> <span dir="rtl">בבסיס הנתונים.</span>

- <span dir="ltr">EF Core Migrations, Database.EnsureCreated</span> <span dir="rtl">או כל מנגנון</span> <span dir="ltr">provisioning</span> <span dir="rtl">אחר.</span>

- <span dir="rtl">שינוי או הרצה של סקריפט ה־</span><span dir="ltr">Python</span> <span dir="rtl">שהקים את בסיס הנתונים.</span>

- <span dir="rtl">מימוש</span> <span dir="ltr">Frontend</span> <span dir="rtl">בתוך פרויקט ה־</span><span dir="ltr">API</span><span dir="rtl">.</span>

- <span dir="rtl">סליקת אשראי העברת כספים אמיתית הפקת קבלות או אינטגרציה בנקאית.</span>

- <span dir="rtl">אחסון מסמכים וקבצים בפועל, אלא אם כבר קיימת לכך תמיכה בסכמה ובשירות חיצוני.</span>

- <span dir="rtl">קביעת משקלי הזכאות וספי הזכאות לפני החלטה עסקית מאושרת.</span>

## **<span dir="rtl">1.4 עקרונות מנחים</span>**

| **<span dir="rtl">עיקרון</span>**<span dir="ltr"></span> | **<span dir="rtl">משמעות</span>**<span dir="ltr"></span> |
|----|----|
| **<span dir="rtl">מקור אמת</span>**<span dir="ltr"></span> | <span dir="ltr">MySQL</span> <span dir="rtl">הקיים הוא מקור האמת לנתונים. ה־</span><span dir="ltr">API</span> <span dir="rtl">ממפה אליו בלבד.</span><span dir="ltr"></span> |
| **<span dir="rtl">הפרדת אחריות</span>**<span dir="ltr"></span> | <span dir="ltr">Controller</span> <span dir="rtl">מטפל ב־</span><span dir="ltr">HTTP, Service</span> <span dir="rtl">בלוגיקה עסקית,</span> <span dir="ltr">Repository</span> <span dir="rtl">בגישה לטבלה ו־</span><span dir="ltr">DbContext</span> <span dir="rtl">במיפוי</span> <span dir="ltr">EF Core.</span> |
| **<span dir="ltr">Repository</span> <span dir="rtl">לכל טבלה</span>**<span dir="ltr"></span> | <span dir="rtl">לכל אחת משבע הטבלאות</span> <span dir="ltr">Repository</span> <span dir="rtl">נפרד. אין גישה ישירה מה־</span><span dir="ltr">Controller</span> <span dir="rtl">או מה־</span><span dir="ltr">Service</span> <span dir="rtl">ל־</span><span dir="ltr">DbContext.</span> |
| **<span dir="rtl">אבטחה כברירת מחדל</span>**<span dir="ltr"></span> | <span dir="rtl">כל פעולה דורשת זהות והרשאה, למעט רישום, התחברות ורשימת סוגי אוכלוסייה פעילים הדרושה לרישום.</span><span dir="ltr"></span> |
| **<span dir="rtl">עקיבות</span>**<span dir="ltr"></span> | <span dir="rtl">פעולות מנהל ושינויי סטטוס חייבים להיות ניתנים לשחזור מלוגים ומחותמות זמן הקיימות בסכמה.</span><span dir="ltr"></span> |
| **<span dir="rtl">אי־תלות ב־</span><span dir="ltr">Frontend</span>**<span dir="ltr"></span> | <span dir="rtl">ה־</span><span dir="ltr">API</span> <span dir="rtl">מחזיר</span> <span dir="ltr">DTOs</span> <span dir="rtl">ושגיאות עקביות ואינו מניח טכנולוגיית</span> <span dir="ltr">UI</span> <span dir="rtl">מסוימת.</span><span dir="ltr"></span> |

<span dir="ltr"></span>

# **<span dir="rtl">2 החלטות סופיות ופערים</span>**

<span dir="rtl">העדכון הנוכחי מאמץ את טבלאות מסמך התכנון המקורי ומתקן הנחות שנוספו באפיון הקודם. החלטות הארכיטקטורה והאיסור לשנות את בסיס הנתונים נשארים בתוקף. פירוט עסקי שלא הוגדר במקור מסומן כהצעה או כהחלטה פתוחה.</span>

| **<span dir="rtl">נושא</span>**<span dir="ltr"></span> | **<span dir="rtl">מצב קודם או פער</span>**<span dir="ltr"></span> | **<span dir="rtl">החלטה באפיון המעודכן</span>**<span dir="ltr"></span> |
|----|----|----|
| **<span dir="ltr">Backend</span> <span dir="rtl">ופריסה</span>**<span dir="ltr"></span> | <span dir="rtl">החלטות הארכיטקטורה נשמרות</span><span dir="ltr"></span> | <span dir="ltr">ASP.NET Core Web API</span> <span dir="rtl">יחיד ו־</span><span dir="ltr">Frontend</span> <span dir="rtl">נפרד.</span><span dir="ltr"></span> |
| **<span dir="rtl">גישה לנתונים</span>**<span dir="ltr"></span> | <span dir="rtl">אין גישה ישירה ל־</span><span dir="ltr">DbContext</span> <span dir="rtl">מהשירותים</span><span dir="ltr"></span> | <span dir="ltr">Controllers</span> <span dir="rtl">אל</span> <span dir="ltr">Services</span> <span dir="rtl">אל</span> <span dir="ltr">Repository</span> <span dir="rtl">לכל טבלה אל</span> <span dir="ltr">DbContext</span> <span dir="rtl">אל</span> <span dir="ltr">MySQL.</span> |
| **<span dir="rtl">זהות והתחברות</span>**<span dir="ltr"></span> | <span dir="rtl">באפיון הקודם שוכפלו שם וסיסמה בכל תפקיד</span><span dir="ltr"></span> | <span dir="rtl">פרטי זהות נשמרים ב־</span><span dir="ltr">User</span> <span dir="rtl">בלבד;</span> <span dir="ltr">Donor, Recipient</span> <span dir="rtl">ו־</span><span dir="ltr">Admin</span> <span dir="rtl">מקושרים ב־</span><span dir="ltr">UserId.</span> |
| **<span dir="rtl">מספר הטבלאות</span>**<span dir="ltr"></span> | <span dir="rtl">באפיון הקודם הופיעו</span> <span dir="ltr">AidRequests</span> <span dir="rtl">ו־</span><span dir="ltr">Allocations</span> | <span dir="rtl">שבע הטבלאות מהמקור; אין טבלאות בקשה והקצאה.</span> <span dir="ltr">User</span> <span dir="rtl">ו־</span><span dir="ltr">PopulationType</span> <span dir="rtl">נכללות במודל.</span><span dir="ltr"></span> |
| **<span dir="rtl">טבלאות רזות</span>**<span dir="ltr"></span> | <span dir="rtl">המקור כולל</span> <span dir="ltr">Id</span> <span dir="rtl">ו־</span><span dir="ltr">UserId</span> <span dir="rtl">בתורם ובמנהל</span><span dir="ltr"></span> | <span dir="ltr">Donor</span> <span dir="rtl">ו־</span><span dir="ltr">Admin</span> <span dir="rtl">כוללות שני שדות בלבד; נתוני הסיוע הייחודיים נמצאים ב־</span><span dir="ltr">Recipient.</span> |
| **<span dir="rtl">סכמה קיימת</span>**<span dir="ltr"></span> | <span dir="rtl">המסמך המקורי אינו בדיקה של ה־</span><span dir="ltr">DB</span> <span dir="rtl">בפועל</span><span dir="ltr"></span> | <span dir="rtl">מיפוי בלבד. אין</span> <span dir="ltr">DDL, migrations</span> <span dir="rtl">או שינוי של סקריפט</span> <span dir="ltr">Python.</span> |
| **<span dir="rtl">סטטוסים</span>**<span dir="ltr"></span> | <span dir="rtl">נוספו בעבר</span> <span dir="ltr">Draft</span> <span dir="rtl">ו־</span><span dir="ltr">Available</span> <span dir="rtl">וסטטוסים נוספים</span><span dir="ltr"></span> | <span dir="rtl">נשמרים ערכי</span> <span dir="ltr">ENUM</span> <span dir="rtl">המקוריים בלבד. משמעות מצב התרומה דורשת אישור כמפורט בסעיף 5.</span><span dir="ltr"></span> |
| **<span dir="rtl">אלגוריתם זכאות</span>**<span dir="ltr"></span> | <span dir="rtl">נוספו שדות ניקוד שלא קיימים במקור</span><span dir="ltr"></span> | <span dir="ltr">EligibilityAmount</span> <span dir="rtl">ו־</span><span dir="ltr">RemainingEligibility</span> <span dir="rtl">בלבד. הנוסחה והמשקלים עדיין לא הוגדרו.</span><span dir="ltr"></span> |
| **<span dir="rtl">פערי המקור</span>**<span dir="ltr"></span> | <span dir="ltr">UserType</span> <span dir="rtl">כולל</span> <span dir="ltr">DONER</span> <span dir="rtl">ו־</span><span dir="ltr">RECIPIEN; DonationId</span> <span dir="rtl">תואר כמזהה תורם</span><span dir="ltr"></span> | <span dir="rtl">מאמתים מול</span> <span dir="ltr">DB</span> <span dir="rtl">וממפים בלי שינוי סכמה.</span> <span dir="ltr">DonationId</span> <span dir="rtl">מקשר לתרומה.</span><span dir="ltr"></span> |
| **<span dir="rtl">שדות בטקסט ללא עמודות</span>**<span dir="ltr"></span> | <span dir="rtl">מיקום ומצב משפחתי מוזכרים בתיאור;</span> <span dir="ltr">Email</span> <span dir="rtl">הוסף באפיון הקודם</span><span dir="ltr"></span> | <span dir="rtl">אינם חלק מחוזה השמירה. אין להוסיף שדות ללא החלטה נפרדת.</span><span dir="ltr"></span> |

<span dir="ltr"></span>

<span dir="ltr"></span>

|  |
|----|
| **<span dir="rtl">השלכה לפיתוח:</span>**<span dir="ltr"></span> <span dir="rtl">אם שם טבלה שדה טיפוס נתון או אילוץ במסמך זה אינם תואמים לסכמת</span> <span dir="ltr">MySQL</span> <span dir="rtl">הקיימת, אין לשנות את בסיס הנתונים. יש להתאים את מיפוי</span> <span dir="ltr">EF Core</span> <span dir="rtl">וה־</span><span dir="ltr">DTOs</span><span dir="rtl">, לתעד את אי־ההתאמה ולהחזיר את הנושא להכרעה.</span><span dir="ltr"></span> |

<span dir="ltr"></span>

# **<span dir="rtl">3 בעלי תפקידים והרשאות עסקיות</span>**

| **<span dir="rtl">תפקיד</span>**<span dir="ltr"></span> | **<span dir="rtl">יכולות עיקריות</span>**<span dir="ltr"></span> | **<span dir="rtl">מגבלות</span>**<span dir="ltr"></span> |
|----|----|----|
| **<span dir="rtl">תורם</span>**<span dir="ltr"></span> | <span dir="rtl">עריכת פרטי</span> <span dir="ltr">User</span> <span dir="rtl">האישיים, יצירת תרומות וצפייה ביתרות ובמימושים מסוכמים.</span><span dir="ltr"></span> | <span dir="rtl">אינו קובע זכאות ואינו רואה נתונים רגישים של נתרמים.</span><span dir="ltr"></span> |
| **<span dir="rtl">נתרם</span>**<span dir="ltr"></span> | <span dir="rtl">עריכת נתוני</span> <span dir="ltr">User</span> <span dir="rtl">ו־</span><span dir="ltr">Recipient</span> <span dir="rtl">המותרים, צפייה בזכאות, בחירת תרומה ובקשת מימוש.</span><span dir="ltr"></span> | <span dir="rtl">אינו משנה תפקיד, סכום זכאות, יתרות או נתוני משתמש אחר.</span><span dir="ltr"></span> |
| **<span dir="rtl">מנהל</span>**<span dir="ltr"></span> | <span dir="rtl">ניהול פעילות חשבונות וסוגי אוכלוסייה, בדיקת נתוני זכאות, טיפול בתרומות ובמימושים ודוחות.</span><span dir="ltr"></span> | <span dir="rtl">אין מסלול ציבורי ליצירת מנהל; אין שינוי סכמה או חריגה מתקרות.</span><span dir="ltr"></span> |

<span dir="ltr"></span>

# **<span dir="rtl">4 מודל הנתונים ושבע הטבלאות</span>**

<span dir="rtl">שמות הטבלאות והשדות להלן נלקחו ממסמך התכנון המקורי.</span> <span dir="ltr">Donors, Recipients</span> <span dir="rtl">ו־</span><span dir="ltr">Admins</span> <span dir="rtl">הם שמות ברבים לממשק ולתצוגה; במקור שמות הטבלאות הם</span> <span dir="ltr">Donor, Recipient</span> <span dir="rtl">ו־</span><span dir="ltr">Admin</span><span dir="rtl">. טבלת</span> <span dir="ltr">User</span> <span dir="rtl">מרכזת את הזהות, הסיסמה והפעילות של החשבון. לכל חשבון תפקיד יחיד ופרופיל תואם אחד. טיפוסים,</span> <span dir="ltr">nullability</span><span dir="rtl">, ברירות מחדל ואילוצים מחייבים אימות מול ה־</span><span dir="ltr">DB</span> <span dir="rtl">הקיים; אין להוסיף עמודות או אילוצים מתוך ה־</span><span dir="ltr">API.</span>

## **4.1 User <span dir="rtl">משתמשים</span>**

<span dir="rtl">טבלת הזהות המשותפת לכל התפקידים.</span> <span dir="ltr">Name, Phone, PasswordHash, CreatedAt, IsActive</span> <span dir="rtl">ו־</span><span dir="ltr">UserType</span> <span dir="rtl">נשמרים כאן בלבד. במקור ערכי</span> <span dir="ltr">UserType</span> <span dir="rtl">נכתבו</span> <span dir="ltr">DONER, ADMIN</span> <span dir="rtl">ו־</span><span dir="ltr">RECIPIEN</span><span dir="rtl">; אלה אינם שמות מתוקנים. אם הם אכן הערכים ב־</span><span dir="ltr">DB</span><span dir="rtl">, יש למפות אותם לשמות</span> <span dir="ltr">DONOR</span> <span dir="rtl">ו־</span><span dir="ltr">RECIPIENT</span> <span dir="rtl">בחוזה ה־</span><span dir="ltr">API</span> <span dir="rtl">בלי לשנות את הנתונים או ה־</span><span dir="ltr">ENUM</span><span dir="rtl">.</span>

| **<span dir="rtl">שדה במקור</span>**<span dir="ltr"></span> | **<span dir="rtl">טיפוס במקור</span>**<span dir="ltr"></span> | **<span dir="rtl">אחריות</span>**<span dir="ltr"></span> | **<span dir="rtl">קשר או כלל</span>**<span dir="ltr"></span> |
|----|----|----|----|
| **Id** | INT | <span dir="rtl">מזהה משתמש</span><span dir="ltr"></span> | PK; Auto Increment |
| **Name** | VARCHAR(50) | <span dir="rtl">שם המשתמש</span><span dir="ltr"></span> | <span dir="rtl">עד 50 תווים; לא מובטחת ייחודיות</span><span dir="ltr"></span> |
| **Phone** | VARCHAR(50) | <span dir="rtl">טלפון</span><span dir="ltr"></span> | <span dir="rtl">נרמול ובדיקת ייחודיות לפי מדיניות התחברות</span><span dir="ltr"></span> |
| **PasswordHash** | VARCHAR(255) | <span dir="rtl">גיבוב סיסמה</span><span dir="ltr"></span> | <span dir="rtl">נשמר כאן בלבד; אינו נחשף בתגובה</span><span dir="ltr"></span> |
| **CreatedAt** | DATETIME | <span dir="rtl">מועד יצירת חשבון</span><span dir="ltr"></span> | <span dir="rtl">נקבע בשרת; יש לברר מוסכמת זמן קיימת</span><span dir="ltr"></span> |
| **IsActive** | BOOLEAN | <span dir="rtl">פעילות החשבון</span><span dir="ltr"></span> | <span dir="rtl">נבדק בכל פעולה רגישה</span><span dir="ltr"></span> |
| **UserType** | ENUM | <span dir="rtl">סוג החשבון</span><span dir="ltr"></span> | <span dir="rtl">ערכי המקור:</span> <span dir="ltr">DONER, ADMIN, RECIPIEN</span> |

<span dir="ltr"></span>

## **4.2 Donor <span dir="rtl">תורמים</span>**

<span dir="rtl">טבלה רזה הכוללת</span> <span dir="ltr">Id</span> <span dir="rtl">ו־</span><span dir="ltr">UserId</span> <span dir="rtl">בלבד. פרטי שם, טלפון, סיסמה ופעילות מגיעים מ־</span><span dir="ltr">User. Donation.DonorId</span> <span dir="rtl">מפנה למזהה הפרופיל</span> <span dir="ltr">Donor.Id</span> <span dir="rtl">ולא למזהה החשבון.</span>

| **<span dir="rtl">שדה במקור</span>**<span dir="ltr"></span> | **<span dir="rtl">טיפוס במקור</span>**<span dir="ltr"></span> | **<span dir="rtl">אחריות</span>**<span dir="ltr"></span> | **<span dir="rtl">קשר או כלל</span>**<span dir="ltr"></span> |
|----|----|----|----|
| **Id** | INT | <span dir="rtl">מזהה פרופיל תורם</span><span dir="ltr"></span> | PK; Auto Increment |
| **UserId** | INT | <span dir="rtl">מזהה החשבון</span><span dir="ltr"></span> | <span dir="ltr">FK</span> <span dir="rtl">אל</span> <span dir="ltr">User.Id</span><span dir="rtl">; קשר לוגי אחד לאחד</span><span dir="ltr"></span> |

<span dir="ltr"></span>

## **4.3 Recipient <span dir="rtl">נתרמים</span>**

<span dir="rtl">פרופיל הנתונים העסקיים של נתרם. סיבת הפנייה ונתוני הזכאות נשמרים כאן, ללא טבלת בקשות נפרדת.</span> <span dir="ltr">EligibilityAmount</span> <span dir="rtl">הוא סכום זכאות ולא ציון;</span> <span dir="ltr">RemainingEligibility</span> <span dir="rtl">הוא הסכום שעוד ניתן לממש. שינוי נתוני המקור אינו מאפס אוטומטית את היתרה.</span>

| **<span dir="rtl">שדה במקור</span>**<span dir="ltr"></span> | **<span dir="rtl">טיפוס במקור</span>**<span dir="ltr"></span> | **<span dir="rtl">אחריות</span>**<span dir="ltr"></span> | **<span dir="rtl">קשר או כלל</span>**<span dir="ltr"></span> |
|----|----|----|----|
| **Id** | INT | <span dir="rtl">מזהה פרופיל נתרם</span><span dir="ltr"></span> | PK; Auto Increment |
| **UserId** | INT | <span dir="rtl">מזהה החשבון</span><span dir="ltr"></span> | <span dir="ltr">FK</span> <span dir="rtl">אל</span> <span dir="ltr">User.Id</span><span dir="rtl">; פרופיל יחיד לחשבון</span><span dir="ltr"></span> |
| **PopulationTypeId** | INT | <span dir="rtl">סוג אוכלוסייה</span><span dir="ltr"></span> | <span dir="ltr">FK</span> <span dir="rtl">אל</span> <span dir="ltr">PopulationType.Id</span> |
| **RequestReason** | VARCHAR(250) | <span dir="rtl">סיבת הפנייה לסיוע</span><span dir="ltr"></span> | <span dir="rtl">עד 250 תווים</span><span dir="ltr"></span> |
| **IncomeAmount** | DECIMAL(10,2) | <span dir="rtl">סכום הכנסה מדווח</span><span dir="ltr"></span> | <span dir="rtl">אפס או יותר; תקופת ההכנסה טעונה הגדרה</span><span dir="ltr"></span> |
| **AmountOfChildren** | INT | <span dir="rtl">מספר ילדים</span><span dir="ltr"></span> | <span dir="rtl">מספר שלם שאינו שלילי</span><span dir="ltr"></span> |
| **EligibilityAmount** | DECIMAL(10,2) | <span dir="rtl">סכום הזכאות</span><span dir="ltr"></span> | <span dir="rtl">נקבע בתהליך מאושר; אינו ציון</span><span dir="ltr"></span> |
| **RemainingEligibility** | DECIMAL(10,2) | <span dir="rtl">יתרת זכאות למימוש</span><span dir="ltr"></span> | <span dir="rtl">בין אפס לסכום הזכאות; עדכון בשרת בלבד</span><span dir="ltr"></span> |

<span dir="ltr"></span>

## **4.4 Admin <span dir="rtl">מנהלים</span>**

<span dir="rtl">טבלה רזה הכוללת</span> <span dir="ltr">Id</span> <span dir="rtl">ו־</span><span dir="ltr">UserId</span> <span dir="rtl">בלבד. פרטי ההתחברות והפעילות נשמרים ב־</span><span dir="ltr">User</span><span dir="rtl">; עצם הפרופיל התואם לתפקיד מעניק הרשאת מנהל. אין שדה מנהל־על או דירוג הרשאות בטבלה המקורית.</span>

| **<span dir="rtl">שדה במקור</span>**<span dir="ltr"></span> | **<span dir="rtl">טיפוס במקור</span>**<span dir="ltr"></span> | **<span dir="rtl">אחריות</span>**<span dir="ltr"></span> | **<span dir="rtl">קשר או כלל</span>**<span dir="ltr"></span> |
|----|----|----|----|
| **Id** | INT | <span dir="rtl">מזהה פרופיל מנהל</span><span dir="ltr"></span> | PK; Auto Increment |
| **UserId** | INT | <span dir="rtl">מזהה החשבון</span><span dir="ltr"></span> | <span dir="ltr">FK</span> <span dir="rtl">אל</span> <span dir="ltr">User.Id</span><span dir="rtl">; תפקיד</span> <span dir="ltr">ADMIN</span> <span dir="rtl">ב־</span><span dir="ltr">User</span> |

<span dir="ltr"></span>

## **4.5 PopulationType <span dir="rtl">סוגי אוכלוסייה</span>**

<span dir="rtl">רשימת סוגי האוכלוסייה במערכת. תרומה ונתרם מקושרים לאותו מילון באמצעות</span> <span dir="ltr">PopulationTypeId</span><span dir="rtl">. אין להחליף אותו בשדה</span> <span dir="ltr">Category</span> <span dir="rtl">חופשי או להוסיף טבלת קטגוריות.</span>

| **<span dir="rtl">שדה במקור</span>**<span dir="ltr"></span> | **<span dir="rtl">טיפוס במקור</span>**<span dir="ltr"></span> | **<span dir="rtl">אחריות</span>**<span dir="ltr"></span> | **<span dir="rtl">קשר או כלל</span>**<span dir="ltr"></span> |
|----|----|----|----|
| **Id** | INT | <span dir="rtl">מזהה סוג אוכלוסייה</span><span dir="ltr"></span> | PK; Auto Increment |
| **Name** | VARCHAR(50) | <span dir="rtl">שם סוג האוכלוסייה</span><span dir="ltr"></span> | <span dir="rtl">עד 50 תווים</span><span dir="ltr"></span> |
| **Description** | VARCHAR(250) | <span dir="rtl">תיאור</span><span dir="ltr"></span> | <span dir="rtl">עד 250 תווים</span><span dir="ltr"></span> |
| **IsActive** | BOOLEAN | <span dir="rtl">זמינות הסוג</span><span dir="ltr"></span> | <span dir="rtl">השבתה מונעת שיוך ומימוש חדשים</span><span dir="ltr"></span> |

<span dir="ltr"></span>

## **4.6 Donation <span dir="rtl">תרומות</span>**

<span dir="rtl">תרומה של</span> <span dir="ltr">Donor</span> <span dir="rtl">המיועדת לסוג אוכלוסייה אחד.</span> <span dir="ltr">RemainingAmount</span> <span dir="rtl">הוא יתרת התרומה;</span> <span dir="ltr">MaxAmountPerRecipient</span> <span dir="rtl">מגביל את הסיוע מאותה תרומה לאותו נתרם. מימוש שהושלם מפחית את היתרה. הצגת תרומה לנתרם אינה שומרת או מקצה סכום.</span>

| **<span dir="rtl">שדה במקור</span>**<span dir="ltr"></span> | **<span dir="rtl">טיפוס במקור</span>**<span dir="ltr"></span> | **<span dir="rtl">אחריות</span>**<span dir="ltr"></span> | **<span dir="rtl">קשר או כלל</span>**<span dir="ltr"></span> |
|----|----|----|----|
| **Id** | INT | <span dir="rtl">מזהה תרומה</span><span dir="ltr"></span> | PK; Auto Increment |
| **DonorId** | INT | <span dir="rtl">פרופיל התורם</span><span dir="ltr"></span> | <span dir="ltr">FK</span> <span dir="rtl">אל</span> <span dir="ltr">Donor.Id</span> |
| **PopulationTypeId** | INT | <span dir="rtl">אוכלוסיית יעד</span><span dir="ltr"></span> | <span dir="ltr">FK</span> <span dir="rtl">אל</span> <span dir="ltr">PopulationType.Id</span> |
| **OriginalAmount** | DECIMAL(10,2) | <span dir="rtl">סכום מקורי</span><span dir="ltr"></span> | <span dir="rtl">חיובי; נקבע ביצירה</span><span dir="ltr"></span> |
| **RemainingAmount** | DECIMAL(10,2) | <span dir="rtl">יתרה למימוש</span><span dir="ltr"></span> | <span dir="rtl">מתחילה בסכום המקורי</span><span dir="ltr"></span> |
| **MaxAmountPerRecipient** | DECIMAL(10,2) | <span dir="rtl">מגבלה לכל נתרם</span><span dir="ltr"></span> | <span dir="rtl">חיובית ולא מעל הסכום המקורי</span><span dir="ltr"></span> |
| **Description** | VARCHAR(250) | <span dir="rtl">תיאור התרומה</span><span dir="ltr"></span> | <span dir="rtl">עד 250 תווים</span><span dir="ltr"></span> |
| **CreatedAt** | DATETIME | <span dir="rtl">מועד יצירה</span><span dir="ltr"></span> | <span dir="rtl">נקבע בשרת</span><span dir="ltr"></span> |
| **ExpirationDate** | DATETIME | <span dir="rtl">מועד פקיעה</span><span dir="ltr"></span> | <span dir="rtl">מאוחר ממועד היצירה</span><span dir="ltr"></span> |
| **IsActive** | BOOLEAN | <span dir="rtl">פעילות התרומה</span><span dir="ltr"></span> | <span dir="rtl">שער זמינות בנוסף לסטטוס ולתוקף</span><span dir="ltr"></span> |
| **Status** | ENUM | <span dir="rtl">מצב עסקי</span><span dir="ltr"></span> | PENDING, COMPLETED, CANCELLED, REJECTED |

<span dir="ltr"></span>

## **4.7 Redemption <span dir="rtl">מימושים</span>**

<span dir="rtl">רשומה המקשרת ישירות בין</span> <span dir="ltr">Donation</span> <span dir="rtl">לבין</span> <span dir="ltr">Recipient</span><span dir="rtl">, עם</span> <span dir="ltr">Admin</span> <span dir="rtl">מטפל לפי התהליך והסכמה. אין</span> <span dir="ltr">AllocationId</span><span dir="rtl">. במקור תיאור</span> <span dir="ltr">DonationId</span> <span dir="rtl">כ״מזהה תורם״ הוא אי־דיוק: השדה מפנה לתרומה. אין במקור</span> <span dir="ltr">ReferenceCode, FailureReason</span> <span dir="rtl">או</span> <span dir="ltr">CompletedAt</span><span dir="rtl">; המועד הקיים הוא</span> <span dir="ltr">RedeemedAt</span><span dir="rtl">.</span>

| **<span dir="rtl">שדה במקור</span>**<span dir="ltr"></span> | **<span dir="rtl">טיפוס במקור</span>**<span dir="ltr"></span> | **<span dir="rtl">אחריות</span>**<span dir="ltr"></span> | **<span dir="rtl">קשר או כלל</span>**<span dir="ltr"></span> |
|----|----|----|----|
| **Id** | INT | <span dir="rtl">מזהה מימוש</span><span dir="ltr"></span> | PK; Auto Increment |
| **DonationId** | INT | <span dir="rtl">תרומה ממומשת</span><span dir="ltr"></span> | <span dir="ltr">FK</span> <span dir="rtl">אל</span> <span dir="ltr">Donation.Id</span> |
| **RecipientId** | INT | <span dir="rtl">פרופיל הנתרם</span><span dir="ltr"></span> | <span dir="ltr">FK</span> <span dir="rtl">אל</span> <span dir="ltr">Recipient.Id</span> |
| **HandledByAdminId** | INT | <span dir="rtl">המנהל המטפל</span><span dir="ltr"></span> | <span dir="ltr">FK</span> <span dir="rtl">אל</span> <span dir="ltr">Admin.Id; nullability</span> <span dir="rtl">טעון בדיקה</span><span dir="ltr"></span> |
| **CreatedAt** | DATETIME | <span dir="rtl">מועד פתיחת מימוש</span><span dir="ltr"></span> | <span dir="rtl">נקבע בשרת</span><span dir="ltr"></span> |
| **RedeemedAt** | DATETIME | <span dir="rtl">מועד השלמה</span><span dir="ltr"></span> | <span dir="rtl">נקבע בהשלמה; מצב לפני השלמה טעון בדיקת סכמה</span><span dir="ltr"></span> |
| **Amount** | DECIMAL(10,2) | <span dir="rtl">סכום מימוש</span><span dir="ltr"></span> | <span dir="rtl">חיובי ובגבולות כל היתרות</span><span dir="ltr"></span> |
| **Status** | ENUM | <span dir="rtl">מצב מימוש</span><span dir="ltr"></span> | PENDING, COMPLETED, CANCELLED, REJECTED |

<span dir="ltr"></span>

## **<span dir="rtl">4.8 קשרים וקרדינליות</span>**

| **<span dir="rtl">קשר</span>**<span dir="ltr"></span> | **<span dir="rtl">קרדינליות</span>**<span dir="ltr"></span> | **<span dir="rtl">משמעות</span>**<span dir="ltr"></span> |
|----|----|----|
| **<span dir="ltr">User</span> <span dir="rtl">אל</span> <span dir="ltr">Donor</span>**<span dir="ltr"></span> | <span dir="ltr"></span><span dir="rtl">1 אל 0 או 1</span><span dir="ltr"></span> | <span dir="rtl">לחשבון תורם פרופיל</span> <span dir="ltr">Donor</span> <span dir="rtl">אחד דרך</span> <span dir="ltr">UserId.</span> |
| **<span dir="ltr">User</span> <span dir="rtl">אל</span> <span dir="ltr">Recipient</span>**<span dir="ltr"></span> | <span dir="ltr"></span><span dir="rtl">1 אל 0 או 1</span><span dir="ltr"></span> | <span dir="rtl">לחשבון נתרם פרופיל</span> <span dir="ltr">Recipient</span> <span dir="rtl">אחד דרך</span> <span dir="ltr">UserId.</span> |
| **<span dir="ltr">User</span> <span dir="rtl">אל</span> <span dir="ltr">Admin</span>**<span dir="ltr"></span> | <span dir="ltr"></span><span dir="rtl">1 אל 0 או 1</span><span dir="ltr"></span> | <span dir="rtl">לחשבון מנהל פרופיל</span> <span dir="ltr">Admin</span> <span dir="rtl">אחד דרך</span> <span dir="ltr">UserId.</span> |
| **<span dir="ltr">Donor</span> <span dir="rtl">אל</span> <span dir="ltr">Donation</span>**<span dir="ltr"></span> | <span dir="rtl">אחד לרבים</span><span dir="ltr"></span> | <span dir="rtl">תרומות התורם מקושרות ל־</span><span dir="ltr">Donor.Id.</span> |
| **<span dir="ltr">PopulationType</span> <span dir="rtl">אל</span> <span dir="ltr">Recipient</span>**<span dir="ltr"></span> | <span dir="rtl">אחד לרבים</span><span dir="ltr"></span> | <span dir="rtl">כל נתרם משויך לסוג אוכלוסייה אחד.</span><span dir="ltr"></span> |
| **<span dir="ltr">PopulationType</span> <span dir="rtl">אל</span> <span dir="ltr">Donation</span>**<span dir="ltr"></span> | <span dir="rtl">אחד לרבים</span><span dir="ltr"></span> | <span dir="rtl">כל תרומה מיועדת לסוג אוכלוסייה אחד.</span><span dir="ltr"></span> |
| **<span dir="ltr">Donation</span> <span dir="rtl">אל</span> <span dir="ltr">Redemption</span>**<span dir="ltr"></span> | <span dir="rtl">אחד לרבים</span><span dir="ltr"></span> | <span dir="rtl">תרומה יכולה להתממש במספר פעולות.</span><span dir="ltr"></span> |
| **<span dir="ltr">Recipient</span> <span dir="rtl">אל</span> <span dir="ltr">Redemption</span>**<span dir="ltr"></span> | <span dir="rtl">אחד לרבים</span><span dir="ltr"></span> | <span dir="rtl">כל מימוש שייך לנתרם אחד.</span><span dir="ltr"></span> |
| **<span dir="ltr">Admin</span> <span dir="rtl">אל</span> <span dir="ltr">Redemption</span>**<span dir="ltr"></span> | <span dir="rtl">אחד לרבים</span><span dir="ltr"></span> | <span dir="rtl">מנהל מטפל במספר מימושים; אפשרות חוסר מנהל זמני לפי הסכמה.</span><span dir="ltr"></span> |

<span dir="ltr"></span>

## **<span dir="rtl">4.9 כללי עקביות בין טבלאות</span>**

- <span dir="rtl">לכל</span> <span dir="ltr">User</span> <span dir="rtl">יש פרופיל אחד בלבד בהתאם ל־</span><span dir="ltr">UserType</span><span dir="rtl">. אין ליצור שני פרופילים לאותו חשבון או פרופיל בתפקיד סותר. יש לבדוק אם קיים אילוץ ייחודיות על</span> <span dir="ltr">UserId</span><span dir="rtl">; אין להוסיף אותו אוטומטית.</span>

- <span dir="ltr">RemainingAmount</span> <span dir="rtl">נמצא בין אפס ל־</span><span dir="ltr">OriginalAmount; RemainingEligibility</span> <span dir="rtl">נמצא בין אפס ל־</span><span dir="ltr">EligibilityAmount</span><span dir="rtl">. כל הסכומים הם ערכי</span> <span dir="ltr">Decimal</span> <span dir="rtl">ולא מספרי נקודה צפה.</span>

- <span dir="rtl">סכום המימושים שהושלמו לאותו צמד</span> <span dir="ltr">DonationId</span> <span dir="rtl">ו־</span><span dir="ltr">RecipientId</span><span dir="rtl">, בצירוף המימוש החדש, אינו עולה על</span> <span dir="ltr">MaxAmountPerRecipient</span><span dir="rtl">. זהו פירוש המגבלה לצורך האפיון, ויש לאשרו לפני הפיתוח.</span>

- <span dir="ltr">DonationId</span> <span dir="rtl">מפנה ל־</span><span dir="ltr">Donation.Id, RecipientId</span> <span dir="rtl">ל־</span><span dir="ltr">Recipient.Id</span> <span dir="rtl">ו־</span><span dir="ltr">HandledByAdminId</span> <span dir="rtl">ל־</span><span dir="ltr">Admin.Id</span><span dir="rtl">. אין לערבב מזהי פרופיל עם</span> <span dir="ltr">User.Id</span><span dir="rtl">.</span>

- <span dir="ltr">PopulationTypeId</span> <span dir="rtl">של התרומה ושל הנתרם חייב להיות זהה וסוג האוכלוסייה פעיל. לא מוגדר מסלול לעקיפת התאמה בידי מנהל.</span>

- <span dir="rtl">השלמת מימוש, הפחתת יתרת התרומה, הפחתת יתרת הזכאות וקביעת</span> <span dir="ltr">RedeemedAt</span> <span dir="rtl">נעשות באותה טרנזקציה, עם בדיקת מצב ויתרות תחת בקרת מקביליות.</span>

# **<span dir="rtl">5 סטטוסים ומעברי מצב</span>**

## **<span dir="rtl">5.1 סטטוס חשבון</span>**

| **<span dir="rtl">ישות</span>**<span dir="ltr"></span> | **<span dir="rtl">מצב</span>**<span dir="ltr"></span> | **<span dir="rtl">משמעות</span>**<span dir="ltr"></span> |
|----|----|----|
| **User** | IsActive = true | <span dir="rtl">חשבון פעיל; פעולה תלויה גם בתפקיד ובתנאים העסקיים.</span><span dir="ltr"></span> |
| **User** | IsActive = false | <span dir="rtl">אין התחברות או פעולות חדשות; ההיסטוריה נשמרת.</span><span dir="ltr"></span> |
| **Donor / Recipient / Admin** | <span dir="rtl">אין</span> <span dir="ltr">Status</span> <span dir="rtl">נפרד</span><span dir="ltr"></span> | <span dir="rtl">הפעילות מגיעה מ־</span><span dir="ltr">User</span><span dir="rtl">. תפקיד יחיד ופרופיל תואם נדרשים.</span><span dir="ltr"></span> |
| **Recipient** | <span dir="rtl">אין סטטוס זכאות נפרד</span><span dir="ltr"></span> | <span dir="rtl">זכאות כספית ויתרה אינן מחליפות תיעוד החלטה.</span><span dir="ltr"></span> |

<span dir="ltr"></span>

## **<span dir="rtl">5.2 סטטוס תרומה</span>**

| **<span dir="rtl">סטטוס במקור</span>**<span dir="ltr"></span> | **<span dir="rtl">משמעות מוצעת לאישור</span>**<span dir="ltr"></span> | **<span dir="rtl">מעברים מוצעים</span>**<span dir="ltr"></span> |
|----|----|----|
| **PENDING** | <span dir="rtl">תרומה פתוחה; זמינה רק אם</span> <span dir="ltr">IsActive</span> <span dir="rtl">פעיל, בתוקף ובעלת יתרה. הפעלה בידי מנהל משמשת לאישור.</span><span dir="ltr"></span> | <span dir="ltr">COMPLETED</span> <span dir="rtl">או</span> <span dir="ltr">CANCELLED; REJECTED</span> <span dir="rtl">לפני מימוש</span><span dir="ltr"></span> |
| **COMPLETED** | <span dir="rtl">היתרה מוצתה; אין מימוש נוסף. אין לפרש כ״מאושרת״ בלי הכרעה עסקית.</span><span dir="ltr"></span> | <span dir="rtl">מצב סופי</span><span dir="ltr"></span> |
| **CANCELLED** | <span dir="rtl">היתרה הלא ממומשת אינה זמינה; ההיסטוריה נשמרת.</span><span dir="ltr"></span> | <span dir="rtl">מצב סופי</span><span dir="ltr"></span> |
| **REJECTED** | <span dir="rtl">התרומה נדחתה לפני שהושלם ממנה מימוש.</span><span dir="ltr"></span> | <span dir="rtl">מצב סופי</span><span dir="ltr"></span> |

<span dir="ltr"></span>

## **<span dir="rtl">5.3 מצב זכאות וזמינות מחושבים</span>**

| **<span dir="rtl">מצב מוצג</span>**<span dir="ltr"></span> | **<span dir="rtl">אופן קביעה</span>**<span dir="ltr"></span> | **<span dir="rtl">משמעות</span>**<span dir="ltr"></span> |
|----|----|----|
| **<span dir="rtl">יתרת זכאות חיובית</span>**<span dir="ltr"></span> | <span dir="ltr">RemainingEligibility</span> <span dir="rtl">גדול מאפס</span><span dir="ltr"></span> | <span dir="rtl">עשויה לאפשר מימוש בכפוף לכל שאר התנאים.</span><span dir="ltr"></span> |
| **<span dir="rtl">אין יתרה למימוש</span>**<span dir="ltr"></span> | <span dir="ltr">RemainingEligibility</span> <span dir="rtl">שווה אפס</span><span dir="ltr"></span> | <span dir="rtl">אין מימוש; לא ניתן להסיק אם נדחה, נוצל או טרם חושב.</span><span dir="ltr"></span> |
| **<span dir="rtl">תרומה פגה</span>**<span dir="ltr"></span> | <span dir="rtl">הגיע</span> <span dir="ltr">ExpirationDate</span> | <span dir="rtl">אין השלמה חדשה; אינו ערך</span> <span dir="ltr">ENUM</span> <span dir="rtl">נוסף.</span><span dir="ltr"></span> |
| **<span dir="rtl">מגבלת נתרם מוצתה</span>**<span dir="ltr"></span> | <span dir="rtl">סך מימושים שהושלמו הגיע למגבלה</span><span dir="ltr"></span> | <span dir="rtl">אין מימוש נוסף מאותה תרומה עבור אותו נתרם.</span><span dir="ltr"></span> |

<span dir="ltr"></span>

## **<span dir="rtl">5.4 סטטוס מימוש</span>**

| **<span dir="rtl">סטטוס מימוש</span>**<span dir="ltr"></span> | **<span dir="rtl">משמעות ומעברים</span>**<span dir="ltr"></span> |
|----|----|
| **PENDING** | <span dir="rtl">בקשה ממתינה; ללא שריון וללא הפחתה. ניתן להשלים, לדחות או לבטל.</span><span dir="ltr"></span> |
| **COMPLETED** | <span dir="rtl">השלמה אטומית והפחתת שתי היתרות פעם אחת. מצב סופי.</span><span dir="ltr"></span> |
| **REJECTED** | <span dir="rtl">דחייה ללא שינוי יתרות; נימוק בלוג. מצב סופי.</span><span dir="ltr"></span> |
| **CANCELLED** | <span dir="rtl">ביטול של בקשה ממתינה בלבד; ללא שינוי יתרות. מצב סופי.</span><span dir="ltr"></span> |

<span dir="ltr"></span>

# **<span dir="rtl">6 תהליכים עסקיים</span>**

## **<span dir="rtl">6.1 רישום והתחברות</span>**

1.  <span dir="rtl">הרישום הציבורי מאפשר תורם או נתרם בלבד. ה־</span><span dir="ltr">Service</span> <span dir="rtl">קובע את התפקיד לפי מסלול הרישום; הלקוח אינו רשאי לבחור</span> <span dir="ltr">ADMIN</span> <span dir="rtl">או לשלוח</span> <span dir="ltr">UserId</span> <span dir="rtl">לחשבון קיים.</span>

2.  <span dir="rtl">נוצרת רשומת</span> <span dir="ltr">User</span> <span dir="rtl">עם גיבוב סיסמה, ולאחריה</span> <span dir="ltr">Donor</span> <span dir="rtl">או</span> <span dir="ltr">Recipient</span> <span dir="rtl">המקושר אליה. שתי הכתיבות מבוצעות בטרנזקציה אחת באמצעות שני</span> <span dir="ltr">Repositories</span><span dir="rtl">; כשל מבטל את שתיהן.</span>

3.  <span dir="ltr">User.IsActive</span> <span dir="rtl">הוא דגל הפעילות היחיד של החשבון. אין ערכי</span> <span dir="ltr">Pending</span> <span dir="rtl">או</span> <span dir="ltr">Suspended</span> <span dir="rtl">בטבלאות התפקיד. מדיניות ההפעלה הראשונית טעונה אישור; עד קביעת זכאות אין לאפשר מימוש לנתרם באמצעות יתרה חיובית שרירותית.</span>

4.  <span dir="rtl">מסלול ההתחברות משותף ומאמת מול</span> <span dir="ltr">User.PasswordHash</span><span dir="rtl">. מזהה החשבון</span> <span dir="ltr">User.Id</span> <span dir="rtl">הוא מזהה חד־משמעי זמין במודל; שימוש ב־</span><span dir="ltr">Phone</span> <span dir="rtl">להתחברות מחייב בירור ייחודיות ונרמול. אין</span> <span dir="ltr">Email</span> <span dir="rtl">במקור ואין להניח ש־</span><span dir="ltr">Name</span> <span dir="rtl">ייחודי. האסימון כולל</span> <span dir="ltr">User.Id</span> <span dir="rtl">ותפקיד, והשרת מאתר את פרופיל התפקיד התואם.</span>

## **<span dir="rtl">6.2 עדכון נתוני סיוע וזכאות</span>**

1.  <span dir="rtl">נתרם מעדכן בפרופיל את</span> <span dir="ltr">PopulationTypeId, RequestReason, IncomeAmount</span> <span dir="rtl">ו־</span><span dir="ltr">AmountOfChildren</span> <span dir="rtl">לפי השדות המותרים.</span>

2.  <span dir="rtl">השרת מאמת את הנתונים וסוג האוכלוסייה. אין יצירת</span> <span dir="ltr">AidRequest</span><span dir="rtl">, מספר בקשה, טיוטה או מועד הגשה נפרד.</span>

3.  <span dir="rtl">מנהל רשאי לבדוק נתונים ולהפעיל חישוב זכאות רק לאחר שאושרו האלגוריתם והמשקלים. מסלול קביעה ידנית יופעל רק אם אושר עסקית.</span>

4.  <span dir="rtl">התוצאה הכספית נשמרת ב־</span><span dir="ltr">EligibilityAmount</span> <span dir="rtl">והיתרה ב־</span><span dir="ltr">RemainingEligibility</span><span dir="rtl">. יש לקבוע מדיניות חישוב מחדש בלי להחזיר סיוע שכבר מומש. החלטות וגרסת הכללים, ככל שתוגדר, מתועדות בלוג חיצוני ולא בשדה שאינו קיים.</span>

## **<span dir="rtl">6.3 יצירת תרומה</span>**

1.  <span dir="rtl">תורם פעיל יוצר תרומה עם</span> <span dir="ltr">PopulationTypeId, OriginalAmount, MaxAmountPerRecipient, Description</span> <span dir="rtl">ו־</span><span dir="ltr">ExpirationDate</span><span dir="rtl">. השרת גוזר</span> <span dir="ltr">DonorId</span> <span dir="rtl">מהחשבון.</span>

2.  <span dir="rtl">השרת קובע</span> <span dir="ltr">CreatedAt</span> <span dir="rtl">ומאתחל</span> <span dir="ltr">RemainingAmount</span> <span dir="rtl">לסכום המקורי. הלקוח אינו קובע יתרה, סטטוס או דגל פעילות בעצמו.</span>

3.  <span dir="rtl">המקור אינו מגדיר סטטוס אישור נפרד. האפיון מציע להשתמש ב־</span><span dir="ltr">PENDING</span> <span dir="rtl">לתרומה פתוחה וב־</span><span dir="ltr">IsActive</span> <span dir="rtl">כשער הזמינות; אישור מנהל יפעיל אותה. משמעות זו דורשת אישור עסקי ומוצגת בסעיף הסטטוסים.</span>

4.  <span dir="rtl">לאחר מימוש ראשון אין לשנות סכום מקורי, סוג אוכלוסייה או מגבלה באופן שיסתור את ההיסטוריה. כל שינוי נבדק מול היתרות והמימושים הקיימים.</span>

## **<span dir="rtl">6.4 התאמת תרומות לנתרם</span>**

1.  <span dir="rtl">נבחרות תרומות פעילות, בתוקף, בעלות יתרה חיובית, בסוג האוכלוסייה של הנתרם ובמצב עסקי פתוח. גם חשבון הנתרם וסוג האוכלוסייה חייבים להיות פעילים.</span>

2.  <span dir="rtl">לכל תרומה מחושב סכום זמין למימוש לפי יתרת התרומה, יתרת זכאות הנתרם ויתרת המגבלה לאותו נתרם. הצגה ברשימה אינה מבטיחה זמינות במועד ההשלמה.</span>

3.  <span dir="rtl">אין רשומת הקצאה או שמירת כסף בשלב ההתאמה. בקשת מימוש ממתינה אינה מפחיתה יתרות ואינה מבטיחה שריון.</span>

4.  <span dir="rtl">סדר תצוגה אינו דירוג זכאות. כלל מיון טכני ניתן להגדרה, אך אין להציגו כמדיניות קדימות עסקית מאושרת.</span>

5.  <span dir="rtl">החלטות תעדוף או חריגה שלא הוגדרו במקור דורשות אישור מפורש; אין להוסיף שדה או טבלה לצורך מימושן.</span>

## **<span dir="rtl">6.5 מימוש</span>**

1.  <span dir="rtl">נתרם מבקש לממש תרומה מסוימת בסכום חיובי.</span> <span dir="ltr">RecipientId</span> <span dir="rtl">נגזר מפרופיל החשבון, ולא מקלט חופשי של הלקוח.</span>

2.  <span dir="rtl">נבדקים פעילות, התאמת אוכלוסייה, תוקף, מצב התרומה, יתרת התרומה, יתרת הזכאות והמגבלה המצטברת לאותו נתרם.</span>

3.  <span dir="rtl">נוצר</span> <span dir="ltr">Redemption</span> <span dir="rtl">במצב</span> <span dir="ltr">PENDING</span><span dir="rtl">. בעת השלמה נבדקים התנאים שוב; הסטטוס משתנה ל־</span><span dir="ltr">COMPLETED</span><span dir="rtl">, נשמרים</span> <span dir="ltr">RedeemedAt</span> <span dir="rtl">והמנהל המטפל, ושתי היתרות מופחתות באותה טרנזקציה. אם</span> <span dir="ltr">HandledByAdminId</span> <span dir="rtl">או</span> <span dir="ltr">RedeemedAt</span> <span dir="rtl">אינם מאפשרים</span> <span dir="ltr">NULL</span><span dir="rtl">, יש להכריע את מסלול ההמתנה מול הסכמה לפני מימושו.</span>

4.  <span dir="rtl">חזרה על השלמה של אותו</span> <span dir="ltr">Redemption.Id</span> <span dir="rtl">מחזירה את תוצאת המימוש הקיים ללא חיוב נוסף. מניעת יצירה כפולה של שתי רשומות שונות אינה נפתרת באמצעות</span> <span dir="ltr">Id</span> <span dir="rtl">בלבד: נדרש מנגנון התמדה מאושר למפתח בקשה, שאינו קיים במודל המקורי.</span>

## **<span dir="rtl">6.6 ביטול תפוגה וסגירה</span>**

1.  <span dir="rtl">ניתן לבטל מימוש</span> <span dir="ltr">PENDING</span> <span dir="rtl">בלי להשיב יתרות, משום שלא הופחתו. ביטול תרומה מונע מימושים עתידיים אך אינו מבטל מימושים שהושלמו.</span>

2.  <span dir="rtl">תפוגה מחושבת מ־</span><span dir="ltr">ExpirationDate</span> <span dir="rtl">ומונעת השלמה חדשה, גם עבור מימוש שהמתין קודם. אין סטטוס</span> <span dir="ltr">EXPIRED</span> <span dir="rtl">ב־</span><span dir="ltr">ENUM</span> <span dir="rtl">המקורי ואין יתרה משוריינת שיש לשחרר.</span>

3.  <span dir="rtl">אין לבטל מימוש שהושלם באמצעות מחיקה; תיקון יתבצע בתהליך היפוך ייעודי אם יוגדר בעתיד.</span>

4.  <span dir="ltr">COMPLETED, CANCELLED</span> <span dir="rtl">ו־</span><span dir="ltr">REJECTED</span> <span dir="rtl">הם מצבים סופיים למימוש. הרשומה נשמרת לצורכי היסטוריה; אין</span> <span dir="ltr">Closed</span> <span dir="rtl">במודל המקורי.</span>

# **<span dir="rtl">7 חוקי זכאות תעדוף ומימוש</span>**

## **<span dir="rtl">7.1 תנאי סף לזכאות</span>**

- <span dir="ltr">User.IsActive</span> <span dir="rtl">של הנתרם פעיל וקיים פרופיל</span> <span dir="ltr">Recipient</span> <span dir="rtl">שתואם ל־</span><span dir="ltr">UserType</span><span dir="rtl">.</span>

- <span dir="ltr">PopulationTypeId</span> <span dir="rtl">מצביע לסוג אוכלוסייה פעיל; סיבת הפנייה ונתוני החישוב הנדרשים תקינים.</span>

- <span dir="rtl">לצורך מימוש נדרשת</span> <span dir="ltr">RemainingEligibility</span> <span dir="rtl">חיובית שנקבעה בתהליך מאושר. רישום כשלעצמו אינו מקנה זכאות.</span>

- <span dir="ltr">IncomeAmount</span> <span dir="rtl">אינו שלילי ו־</span><span dir="ltr">AmountOfChildren</span> <span dir="rtl">הוא מספר שלם שאינו שלילי. אין להסיק ממנו את מספר הנפשות במשק הבית.</span>

- <span dir="rtl">כללי תקופת זכאות, צינון, סיוע חוזר וחישוב מחדש טרם הוגדרו. אין לקבע להם ערכי ברירת מחדל עסקיים.</span>

- <span dir="rtl">כאשר חסר נתון או שהנוסחה אינה מאושרת, אין לנחש זכאות. מצב ״טרם נקבעה זכאות״ אינו נשמר כסטטוס נפרד במקור, ויתרה אפס לבדה אינה מוכיחה דחייה.</span>

## **<span dir="rtl">7.2 נתוני אלגוריתם הזכאות</span>**

<span dir="rtl">האלגוריתם אמור להפיק סכום זכאות מתוך הנתונים הקיימים, אך המשקלים, הנוסחה והספים טרם הוגדרו. המקור כולל הכנסה, מספר ילדים, סוג אוכלוסייה וסיבת פנייה. נתונים אחרים שנכללו באפיון הקודם אינם שדות קיימים ואינם יכולים להפוך לדרישות חובה ללא החלטה חדשה.</span>

| **<span dir="rtl">נתון</span>**<span dir="ltr"></span> | **<span dir="rtl">מקור</span>**<span dir="ltr"></span> | **<span dir="rtl">שימוש אפשרי</span>**<span dir="ltr"></span> | **<span dir="rtl">משקל</span>**<span dir="ltr"></span> |
|----|----|----|----|
| **IncomeAmount** | Recipient | <span dir="rtl">הכנסה; יחידת הזמן ושיטת הנרמול טרם נקבעו</span><span dir="ltr"></span> | <span dir="rtl">לא הוגדר</span><span dir="ltr"></span> |
| **AmountOfChildren** | Recipient | <span dir="rtl">מספר ילדים; אינו זהה למספר נפשות</span><span dir="ltr"></span> | <span dir="rtl">לא הוגדר</span><span dir="ltr"></span> |
| **PopulationTypeId** | Recipient | <span dir="rtl">קבוצת אוכלוסייה והתאמה לתרומה</span><span dir="ltr"></span> | <span dir="rtl">לא הוגדר</span><span dir="ltr"></span> |
| **RequestReason** | Recipient | <span dir="rtl">תיאור הצורך; אין מיפוי מספרי מאושר לטקסט</span><span dir="ltr"></span> | <span dir="rtl">לא הוגדר</span><span dir="ltr"></span> |
| **<span dir="rtl">מימושים קודמים</span>**<span dir="ltr"></span> | Redemption | <span dir="rtl">מידע היסטורי אפשרי; תקופת החישוב טרם נקבעה</span><span dir="ltr"></span> | <span dir="rtl">לא הוגדר</span><span dir="ltr"></span> |

<span dir="ltr"></span>

<span dir="ltr"></span>

|  |
|----|
| **<span dir="rtl">משקלי אלגוריתם הזכאות עדיין לא הוגדרו. גם הנוסחה, הספים, תקופת ההכנסה והזכאות ומדיניות החישוב מחדש פתוחים. אין להפיק זכאות אוטומטית מחייבת, להמציא ציון או להוסיף שדות חסרים. קביעה ידנית מחייבת אישור מדיניות ותיעוד.</span>**<span dir="ltr"></span> |

<span dir="ltr"></span>

## **<span dir="rtl">7.3 כללי תעדוף</span>**

- <span dir="rtl">לא נקבעה מדיניות תעדוף מחייבת בין נתרמים או בין בקשות מימוש. אין במודל שדה</span> <span dir="ltr">EligibilityScore</span><span dir="rtl">.</span>

- <span dir="rtl">ניתן להציג תרומות לפי תוקף או תאריך יצירה, אך יש להבחין בין מיון תצוגה לבין החלטה מי יקבל סיוע קודם.</span>

- <span dir="rtl">אם יוגדר בעתיד תעדוף, יש לאשר גם שוברי שוויון ואת מקור נתוני החישוב. אין להסתמך על</span> <span dir="ltr">SubmittedAt</span> <span dir="rtl">או</span> <span dir="ltr">UrgencyLevel</span> <span dir="rtl">שאינם קיימים במקור.</span>

- <span dir="rtl">למנהל אין היתר מובנה לעקוף תקרות או ליצור יתרה שלילית. כל הרחבת מדיניות דורשת החלטה ותיעוד לפני יישום.</span>

## **<span dir="rtl">7.4 חוקי מימוש</span>**

- <span dir="rtl">הנתרם רשאי לבקש מימוש עבור עצמו בלבד; מנהל רשאי לטפל במימושים בהתאם להרשאה ולתיעוד.</span>

- <span dir="rtl">התרומה והחשבונות הרלוונטיים פעילים, התרומה פתוחה ובתוקף, וסוגי האוכלוסייה תואמים.</span>

- <span dir="rtl">הסכום אינו עולה על המינימום מבין</span> <span dir="ltr">RemainingAmount, RemainingEligibility</span> <span dir="rtl">ו־</span><span dir="ltr">MaxAmountPerRecipient</span> <span dir="rtl">פחות סך מימושי</span> <span dir="ltr">COMPLETED</span> <span dir="rtl">לאותו צמד תרומה ונתרם.</span>

- <span dir="rtl">מימוש</span> <span dir="ltr">COMPLETED</span> <span dir="rtl">אינו ניתן למחיקה או לעריכה כספית ישירה. תיקון היסטוריה דורש תהליך נפרד שלא נכלל בגרסה זו.</span>

- <span dir="rtl">השלמה חוזרת לאותו מזהה אינה משנה שוב יתרות. ההגנה מפני יצירת שתי בקשות זהות נפרדות היא פער פתוח, ואין לטעון שקיים</span> <span dir="ltr">ReferenceCode</span><span dir="rtl">.</span>

- <span dir="rtl">בדיקת המצב והיתרות, שתי ההפחתות ועדכון המימוש מתבצעים בטרנזקציה אחת. כשל מבטל הכול; בקשות מקבילות נבדקות תחת נעילה או עדכון מותנה התואמים לסכמה.</span>

# **<span dir="rtl">8 ולידציות וכללי עקביות</span>**

| **<span dir="rtl">תחום</span>**<span dir="ltr"></span> | **<span dir="rtl">כללים</span>**<span dir="ltr"></span> |
|----|----|
| **<span dir="rtl">קלט ואורכים</span>**<span dir="ltr"></span> | <span dir="ltr">Name</span> <span dir="rtl">ו־</span><span dir="ltr">Phone</span> <span dir="rtl">עד 50 תווים; תיאורים ו־</span><span dir="ltr">RequestReason</span> <span dir="rtl">עד 250;</span> <span dir="ltr">DTO</span> <span dir="rtl">ייעודי לכל פעולה. חובה ו־</span><span dir="ltr">NULL</span> <span dir="rtl">נבדקים מול הסכמה.</span><span dir="ltr"></span> |
| **<span dir="rtl">זהות</span>**<span dir="ltr"></span> | <span dir="ltr">PasswordHash</span> <span dir="rtl">ב־</span><span dir="ltr">User</span> <span dir="rtl">בלבד. אין</span> <span dir="ltr">Email</span> <span dir="rtl">במקור. אין שימוש ב־</span><span dir="ltr">Name</span> <span dir="rtl">כמזהה ייחודי; התחברות בטלפון דורשת מדיניות ייחודיות.</span><span dir="ltr"></span> |
| **<span dir="rtl">תפקיד ופרופיל</span>**<span dir="ltr"></span> | <span dir="ltr">UserType</span> <span dir="rtl">נקבע בשרת; פרופיל אחד תואם לכל</span> <span dir="ltr">User</span><span dir="rtl">. שדות</span> <span dir="ltr">UserId</span><span dir="rtl">, יתרות וזכאות אינם נתונים לעדכון חופשי.</span><span dir="ltr"></span> |
| **<span dir="rtl">סכומים</span>**<span dir="ltr"></span> | <span dir="rtl">דיוק</span> <span dir="ltr">DECIMAL</span><span dir="rtl">(10,2), סכום מימוש חיובי, הכנסה ויתרות לא שליליות. אין עיגול שקט או חריגה מתקרות.</span><span dir="ltr"></span> |
| **<span dir="rtl">תאריכים</span>**<span dir="ltr"></span> | <span dir="ltr">ExpirationDate</span> <span dir="rtl">מאוחר מ־</span><span dir="ltr">CreatedAt</span><span dir="rtl">; אין השלמה אחרי פקיעה. מוסכמת זמן מאומתת מול נתונים קיימים.</span><span dir="ltr"></span> |
| **<span dir="rtl">בעלות</span>**<span dir="ltr"></span> | <span dir="rtl">נתרם פועל דרך</span> <span dir="ltr">Recipient</span> <span dir="rtl">הקשור ל־</span><span dir="ltr">User</span> <span dir="rtl">שלו; תורם דרך</span> <span dir="ltr">Donor</span><span dir="rtl">. מזהי פרופיל שונים ממזהה החשבון.</span><span dir="ltr"></span> |
| **<span dir="rtl">קשרים</span>**<span dir="ltr"></span> | <span dir="rtl">כל</span> <span dir="ltr">FK</span> <span dir="rtl">מצביע לישות קיימת ומתאימה;</span> <span dir="ltr">PopulationTypeId</span> <span dir="rtl">תואם בין תרומה לנתרם.</span><span dir="ltr"></span> |
| **<span dir="rtl">סטטוסים</span>**<span dir="ltr"></span> | <span dir="rtl">רק ארבעת ערכי המקור; מעברים כספיים נעשים דרך פעולות ייעודיות ולא</span> <span dir="ltr">CRUD</span> <span dir="rtl">חופשי.</span><span dir="ltr"></span> |
| **<span dir="rtl">מקביליות</span>**<span dir="ltr"></span> | <span dir="rtl">נעילה או עדכון מותנה יחד עם טרנזקציה; בדיקה חוזרת של שתי היתרות והמגבלה המצטברת.</span><span dir="ltr"></span> |
| **<span dir="rtl">כפילויות</span>**<span dir="ltr"></span> | <span dir="rtl">השלמה חוזרת של אותו מימוש אינה מחייבת שוב. מניעת יצירת בקשות נפרדות כפולות דורשת פתרון מאושר.</span><span dir="ltr"></span> |
| **<span dir="rtl">מחיקה</span>**<span dir="ltr"></span> | <span dir="rtl">אין מחיקת היסטוריה; השבתת</span> <span dir="ltr">User</span> <span dir="rtl">אינה מוחקת את פרופיל התפקיד, תרומותיו או מימושיו.</span><span dir="ltr"></span> |

<span dir="ltr"></span>

## **<span dir="rtl">8.1 קודי שגיאה והתנהגות</span>**

| **<span dir="rtl">תגובה</span>**<span dir="ltr"></span> | **<span dir="rtl">שימוש</span>**<span dir="ltr"></span> |
|----|----|
| <span dir="ltr">**400 Bad Request**</span> | <span dir="rtl">קלט לא תקין או כלל עסקי בסיסי שנכשל.</span><span dir="ltr"></span> |
| <span dir="ltr">**401 Unauthorized**</span> | <span dir="rtl">אין זהות תקפה.</span><span dir="ltr"></span> |
| <span dir="ltr">**403 Forbidden**</span> | <span dir="rtl">למשתמש אין תפקיד או בעלות מתאימים.</span><span dir="ltr"></span> |
| <span dir="ltr">**404 Not Found**</span> | <span dir="rtl">המשאב לא נמצא או שאינו חשוף למבקש.</span><span dir="ltr"></span> |
| <span dir="ltr">**409 Conflict**</span> | <span dir="rtl">כפילות, גרסת נתון ישנה, יתרה השתנתה או מעבר סטטוס מתנגש.</span><span dir="ltr"></span> |
| <span dir="ltr">**422 Unprocessable Entity**</span> | <span dir="rtl">הקלט תקין תחבירית אך אינו עומד בכללים העסקיים המפורטים.</span><span dir="ltr"></span> |
| <span dir="ltr">**500 Internal Server Error**</span> | <span dir="rtl">תקלה לא צפויה עם מזהה</span> <span dir="ltr">Correlation</span> <span dir="rtl">ללא חשיפת פרטים פנימיים.</span><span dir="ltr"></span> |

<span dir="ltr"></span>

# **<span dir="rtl">9 אבטחה והרשאות</span>**

<span dir="rtl">האימות המוצע הוא</span> <span dir="ltr">Bearer Token</span> <span dir="rtl">מבוסס</span> <span dir="ltr">JWT</span><span dir="rtl">. זהות המשתמש היא</span> <span dir="ltr">User.Id</span><span dir="rtl">, והתפקיד נגזר מ־</span><span dir="ltr">UserType</span> <span dir="rtl">ומפרופיל תואם.</span> <span dir="ltr">Donor.Id, Recipient.Id</span> <span dir="rtl">ו־</span><span dir="ltr">Admin.Id</span> <span dir="rtl">הם מזהי פרופיל נפרדים. בכל פעולה רגישה נבדקים פעילות החשבון והקשר לפרופיל בשרת, גם אם האסימון טרם פג.</span>

| **<span dir="rtl">פעולה</span>**<span dir="ltr"></span> | **<span dir="rtl">תורם</span>**<span dir="ltr"></span> | **<span dir="rtl">נתרם</span>**<span dir="ltr"></span> | **<span dir="rtl">מנהל</span>**<span dir="ltr"></span> |
|----|----|----|----|
| **<span dir="rtl">קריאה ועריכת פרטים עצמיים</span>**<span dir="ltr"></span> | <span dir="rtl">כן</span><span dir="ltr"></span> | <span dir="rtl">כן</span><span dir="ltr"></span> | <span dir="rtl">כן</span><span dir="ltr"></span> |
| **<span dir="rtl">יצירת תרומה</span>**<span dir="ltr"></span> | <span dir="rtl">כן</span><span dir="ltr"></span> | <span dir="rtl">לא</span><span dir="ltr"></span> | <span dir="rtl">לא בשם תורם ללא מסלול שאושר</span><span dir="ltr"></span> |
| **<span dir="rtl">עדכון נתוני סיוע</span>**<span dir="ltr"></span> | <span dir="rtl">לא</span><span dir="ltr"></span> | <span dir="rtl">כן בפרופיל שלו</span><span dir="ltr"></span> | <span dir="rtl">כן עם תיעוד</span><span dir="ltr"></span> |
| **<span dir="rtl">שינוי סכום זכאות</span>**<span dir="ltr"></span> | <span dir="rtl">לא</span><span dir="ltr"></span> | <span dir="rtl">לא</span><span dir="ltr"></span> | <span dir="rtl">רק לפי מדיניות מאושרת</span><span dir="ltr"></span> |
| **<span dir="rtl">בקשת מימוש</span>**<span dir="ltr"></span> | <span dir="rtl">לא</span><span dir="ltr"></span> | <span dir="rtl">כן עבור עצמו</span><span dir="ltr"></span> | <span dir="rtl">טיפול במימוש קיים</span><span dir="ltr"></span> |
| **<span dir="rtl">השלמת או דחיית מימוש</span>**<span dir="ltr"></span> | <span dir="rtl">לא</span><span dir="ltr"></span> | <span dir="rtl">לא</span><span dir="ltr"></span> | <span dir="rtl">כן</span><span dir="ltr"></span> |
| **<span dir="rtl">ניהול פעילות חשבונות</span>**<span dir="ltr"></span> | <span dir="rtl">לא</span><span dir="ltr"></span> | <span dir="rtl">לא</span><span dir="ltr"></span> | <span dir="rtl">כן; הגנות על מנהלים לפי מדיניות</span><span dir="ltr"></span> |
| **<span dir="rtl">ניהול סוגי אוכלוסייה</span>**<span dir="ltr"></span> | <span dir="rtl">לא</span><span dir="ltr"></span> | <span dir="rtl">לא</span><span dir="ltr"></span> | <span dir="rtl">כן</span><span dir="ltr"></span> |
| **<span dir="rtl">שינוי תפקיד או הקמת מנהל</span>**<span dir="ltr"></span> | <span dir="rtl">לא</span><span dir="ltr"></span> | <span dir="rtl">לא</span><span dir="ltr"></span> | <span dir="rtl">אין מסלול פתוח עד אישור מדיניות</span><span dir="ltr"></span> |
| **<span dir="rtl">שינוי סכמת</span> <span dir="ltr">DB</span>**<span dir="ltr"></span> | <span dir="rtl">לא</span><span dir="ltr"></span> | <span dir="rtl">לא</span><span dir="ltr"></span> | <span dir="rtl">לא</span><span dir="ltr"></span> |

<span dir="ltr"></span>

- <span dir="rtl">הסיסמאות נשמרות כ־</span><span dir="ltr">hash</span> <span dir="rtl">חזק עם</span> <span dir="ltr">salt</span> <span dir="rtl">באמצעות מנגנון מוכר; אין לכתוב אלגוריתם הצפנה עצמאי.</span>

- <span dir="rtl">ה־</span><span dir="ltr">API</span> <span dir="rtl">משתמש ב־</span><span dir="ltr">HTTPS</span> <span dir="rtl">בלבד בסביבות שאינן מקומיות.</span>

- <span dir="rtl">סודות כגון</span> <span dir="ltr">connection string</span> <span dir="rtl">ומפתח חתימה נשמרים במשתני סביבה או מנהל סודות ולא בקוד.</span>

- <span dir="rtl">תגובה למשתמש אינה חושפת</span> <span dir="ltr">PasswordHash, connection string, stack trace</span> <span dir="rtl">או פרטי נתרם שאינם נדרשים.</span>

- <span dir="rtl">הרשאה נבדקת גם לפי תפקיד וגם לפי בעלות; סינון ב־</span><span dir="ltr">UI</span> <span dir="rtl">אינו תחליף לאכיפה בשרת.</span>

- <span dir="rtl">פעולות מנהל רגישות כוללות מזהה מבצע, זמן, סיבה ו־</span><span dir="ltr">CorrelationId</span> <span dir="rtl">בלוג ביקורת.</span>

# **<span dir="rtl">10 ממשק</span> REST API**

## **<span dir="rtl">10.1 כללי חוזה</span>**

- <span dir="ltr">Base URL</span> <span dir="rtl">לוגי: ‎/</span><span dir="ltr">api/v</span><span dir="rtl">1.</span>

- <span dir="rtl">קלט ופלט ב־</span><span dir="ltr">JSON</span><span dir="rtl">; שמות מאפיינים עקביים לפי</span> <span dir="ltr">convention</span> <span dir="rtl">שייבחר לפרויקט.</span>

- <span dir="rtl">אין החזרת</span> <span dir="ltr">Entities</span> <span dir="rtl">של</span> <span dir="ltr">EF Core</span> <span dir="rtl">ישירות; משתמשים ב־</span><span dir="ltr">DTOs</span> <span dir="rtl">ייעודיים.</span>

- <span dir="rtl">רשימות תומכות</span> <span dir="ltr">page pageSize sort</span> <span dir="rtl">ו־</span><span dir="ltr">filters; pageSize</span> <span dir="rtl">מוגבל.</span>

- <span dir="rtl">שגיאות מוחזרות במבנה</span> <span dir="ltr">Problem Details</span> <span dir="rtl">עם</span> <span dir="ltr">code message details</span> <span dir="rtl">ו־</span><span dir="ltr">correlationId</span><span dir="rtl">.</span>

- <span dir="rtl">פעולות משנות מצב משתמשות בנתיבי פעולה מפורשים ולא בעדכון</span> <span dir="ltr">Status</span> <span dir="rtl">חופשי.</span>

- <span dir="rtl">פעולת</span> <span dir="ltr">complete</span> <span dir="rtl">חוזרת לאותו מזהה מימוש היא</span> <span dir="ltr">idempotent</span><span dir="rtl">. דרישת</span> <span dir="ltr">Idempotency-Key</span> <span dir="rtl">ליצירת מימוש תופעל רק לאחר שנקבע פתרון התמדה תואם למגבלת אי־שינוי ה־</span><span dir="ltr">DB</span><span dir="rtl">.</span>

- <span dir="rtl">אין מחיקה פיזית של היסטוריה. חשבונות וסוגי אוכלוסייה מושבתים באמצעות</span> <span dir="ltr">IsActive</span><span dir="rtl">; תרומות ומימושים מבוטלים באמצעות פעולות ייעודיות.</span>

## **<span dir="rtl">10.2 אימות וחשבון</span>**

| **<span dir="ltr">Method</span> <span dir="rtl">ונתיב</span>**<span dir="ltr"></span> | **<span dir="rtl">הרשאה</span>**<span dir="ltr"></span> | **<span dir="rtl">מטרה</span>**<span dir="ltr"></span> |
|----|----|----|
| **POST /api/v1/auth/donors/register** | <span dir="rtl">ציבורי</span><span dir="ltr"></span> | <span dir="rtl">יצירת</span> <span dir="ltr">User</span> <span dir="rtl">ו־</span><span dir="ltr">Donor</span> <span dir="rtl">באותה טרנזקציה.</span><span dir="ltr"></span> |
| **POST /api/v1/auth/recipients/register** | <span dir="rtl">ציבורי</span><span dir="ltr"></span> | <span dir="rtl">יצירת</span> <span dir="ltr">User</span> <span dir="rtl">ו־</span><span dir="ltr">Recipient</span> <span dir="rtl">באותה טרנזקציה.</span><span dir="ltr"></span> |
| **POST /api/v1/auth/login** | <span dir="rtl">ציבורי</span><span dir="ltr"></span> | <span dir="rtl">אימות משותף מול</span> <span dir="ltr">User</span><span dir="rtl">; ללא הרשמה ציבורית למנהל.</span><span dir="ltr"></span> |
| **GET /api/v1/users/me** | <span dir="rtl">מזוהה</span><span dir="ltr"></span> | <span dir="rtl">פרטי</span> <span dir="ltr">User</span> <span dir="rtl">ופרופיל תפקיד ללא</span> <span dir="ltr">PasswordHash.</span> |
| **PATCH /api/v1/users/me** | <span dir="rtl">מזוהה</span><span dir="ltr"></span> | <span dir="rtl">עדכון</span> <span dir="ltr">Name</span> <span dir="rtl">ו־</span><span dir="ltr">Phone</span> <span dir="rtl">בלבד לפי מדיניות.</span><span dir="ltr"></span> |
| **POST /api/v1/users/me/change-password** | <span dir="rtl">מזוהה</span><span dir="ltr"></span> | <span dir="rtl">אימות סיסמה נוכחית ועדכון הגיבוב ב־</span><span dir="ltr">User.</span> |
| **GET /api/v1/users** | <span dir="rtl">מנהל</span><span dir="ltr"></span> | <span dir="rtl">רשימת חשבונות מסוננת.</span><span dir="ltr"></span> |
| **PATCH /api/v1/users/{id}/activity** | <span dir="rtl">מנהל</span><span dir="ltr"></span> | <span dir="rtl">עדכון</span> <span dir="ltr">IsActive</span> <span dir="rtl">עם תיעוד ובדיקת הרשאה.</span><span dir="ltr"></span> |

<span dir="ltr"></span>

## **<span dir="rtl">10.3 תורמים ותרומות</span>**

| **<span dir="ltr">Method</span> <span dir="rtl">ונתיב</span>**<span dir="ltr"></span> | **<span dir="rtl">הרשאה</span>**<span dir="ltr"></span> | **<span dir="rtl">מטרה</span>**<span dir="ltr"></span> |
|----|----|----|
| **GET /api/v1/donors** | <span dir="rtl">מנהל</span><span dir="ltr"></span> | <span dir="rtl">רשימת פרופילי</span> <span dir="ltr">Donor</span> <span dir="rtl">עם פרטי</span> <span dir="ltr">User</span> <span dir="rtl">מותרים.</span><span dir="ltr"></span> |
| **GET /api/v1/donors/{id}** | <span dir="rtl">מנהל או עצמי</span><span dir="ltr"></span> | <span dir="rtl">פרופיל תורם;</span> <span dir="ltr">id</span> <span dir="rtl">הוא</span> <span dir="ltr">Donor.Id.</span> |
| **GET /api/v1/donations** | <span dir="rtl">לפי תפקיד</span><span dir="ltr"></span> | <span dir="rtl">תרומות אישיות או זמינות לפי הרשאה.</span><span dir="ltr"></span> |
| **POST /api/v1/donations** | <span dir="rtl">תורם</span><span dir="ltr"></span> | <span dir="rtl">יצירת תרומה;</span> <span dir="ltr">DonorId</span> <span dir="rtl">נגזר מהזהות.</span><span dir="ltr"></span> |
| **GET /api/v1/donations/{id}** | <span dir="rtl">לפי תפקיד</span><span dir="ltr"></span> | <span dir="rtl">פרטי תרומה ללא חשיפת נתוני נתרמים.</span><span dir="ltr"></span> |
| **PATCH /api/v1/donations/{id}** | <span dir="rtl">בעלים</span><span dir="ltr"></span> | <span dir="rtl">עריכת שדות מותרים בלי לפגוע בהיסטוריה או ביתרות.</span><span dir="ltr"></span> |
| **POST /api/v1/donations/{id}/activate** | <span dir="rtl">מנהל</span><span dir="ltr"></span> | <span dir="rtl">הפעלה לפי פירוש הסטטוסים המוצע לאישור.</span><span dir="ltr"></span> |
| **POST /api/v1/donations/{id}/reject** | <span dir="rtl">מנהל</span><span dir="ltr"></span> | <span dir="rtl">דחייה לפני מימוש עם תיעוד.</span><span dir="ltr"></span> |
| **POST /api/v1/donations/{id}/cancel** | <span dir="rtl">בעלים או מנהל</span><span dir="ltr"></span> | <span dir="rtl">מניעת מימושים עתידיים ושמירת ההיסטוריה.</span><span dir="ltr"></span> |

<span dir="ltr"></span>

## **<span dir="rtl">10.4 נתרמים וזכאות</span>**

| **<span dir="ltr">Method</span> <span dir="rtl">ונתיב</span>**<span dir="ltr"></span> | **<span dir="rtl">הרשאה</span>**<span dir="ltr"></span> | **<span dir="rtl">מטרה</span>**<span dir="ltr"></span> |
|----|----|----|
| **GET /api/v1/recipients** | <span dir="rtl">מנהל</span><span dir="ltr"></span> | <span dir="rtl">חיפוש פרופילי</span> <span dir="ltr">Recipient.</span> |
| **GET /api/v1/recipients/{id}** | <span dir="rtl">מנהל או עצמי</span><span dir="ltr"></span> | <span dir="rtl">פרטי</span> <span dir="ltr">User</span> <span dir="rtl">ופרטי הסיוע המותרים.</span><span dir="ltr"></span> |
| **PATCH /api/v1/recipients/me** | <span dir="rtl">נתרם</span><span dir="ltr"></span> | <span dir="rtl">עדכון סוג אוכלוסייה, סיבה, הכנסה ומספר ילדים.</span><span dir="ltr"></span> |
| **GET /api/v1/recipients/me/eligibility** | <span dir="rtl">נתרם</span><span dir="ltr"></span> | <span dir="rtl">סכום ויתרת זכאות; בלי סטטוס מומצא.</span><span dir="ltr"></span> |
| **GET /api/v1/recipients/me/eligible-donations** | <span dir="rtl">נתרם</span><span dir="ltr"></span> | <span dir="rtl">תרומות תואמות ותקרה זמינה בעת השאילתה.</span><span dir="ltr"></span> |
| **POST /api/v1/recipients/{id}/evaluate** | <span dir="rtl">מנהל</span><span dir="ltr"></span> | <span dir="rtl">חישוב זכאות; חסום עד אישור אלגוריתם ומשקלים.</span><span dir="ltr"></span> |
| **PUT /api/v1/recipients/{id}/eligibility** | <span dir="rtl">מנהל</span><span dir="ltr"></span> | <span dir="rtl">מסלול ידני מותנה באישור מדיניות; אין איפוס יתרה אוטומטי.</span><span dir="ltr"></span> |

<span dir="ltr"></span>

## **<span dir="rtl">10.5 מימושים וסוגי אוכלוסייה</span>**

| **<span dir="ltr">Method</span> <span dir="rtl">ונתיב</span>**<span dir="ltr"></span> | **<span dir="rtl">הרשאה</span>**<span dir="ltr"></span> | **<span dir="rtl">מטרה</span>**<span dir="ltr"></span> |
|----|----|----|
| **GET /api/v1/redemptions** | <span dir="rtl">מנהל או עצמי</span><span dir="ltr"></span> | <span dir="rtl">רשימת מימושים לפי בעלות; תורם מקבל רק מידע מצומצם על תרומותיו.</span><span dir="ltr"></span> |
| **POST /api/v1/donations/{id}/redemptions** | <span dir="rtl">נתרם</span><span dir="ltr"></span> | <span dir="rtl">בקשת סכום מימוש;</span> <span dir="ltr">RecipientId</span> <span dir="rtl">נגזר מהזהות.</span><span dir="ltr"></span> |
| **GET /api/v1/redemptions/{id}** | <span dir="rtl">מנהל או צד מורשה</span><span dir="ltr"></span> | <span dir="rtl">פרטי מימוש לפי הרשאה.</span><span dir="ltr"></span> |
| **POST /api/v1/redemptions/{id}/complete** | <span dir="rtl">מנהל</span><span dir="ltr"></span> | <span dir="rtl">השלמה אטומית ועדכון שתי היתרות; בטוח לחזרה על אותו מזהה.</span><span dir="ltr"></span> |
| **POST /api/v1/redemptions/{id}/reject** | <span dir="rtl">מנהל</span><span dir="ltr"></span> | <span dir="rtl">דחיית</span> <span dir="ltr">PENDING</span> <span dir="rtl">בלבד.</span><span dir="ltr"></span> |
| **POST /api/v1/redemptions/{id}/cancel** | <span dir="rtl">נתרם בעלים או מנהל</span><span dir="ltr"></span> | <span dir="rtl">ביטול</span> <span dir="ltr">PENDING</span> <span dir="rtl">בלבד.</span><span dir="ltr"></span> |
| **GET /api/v1/population-types** | <span dir="rtl">לפי שימוש</span><span dir="ltr"></span> | <span dir="rtl">רשימת סוגים פעילים לרישום ולהתאמה; מנהל רשאי לראות גם מושבתים.</span><span dir="ltr"></span> |
| **POST /api/v1/population-types** | <span dir="rtl">מנהל</span><span dir="ltr"></span> | <span dir="rtl">הוספת רשומת סוג אוכלוסייה, לא שינוי סכמה.</span><span dir="ltr"></span> |
| **PATCH /api/v1/population-types/{id}** | <span dir="rtl">מנהל</span><span dir="ltr"></span> | <span dir="rtl">עדכון שם, תיאור ופעילות.</span><span dir="ltr"></span> |

<span dir="ltr"></span>

## **<span dir="rtl">10.6 מנהלים ובקרה</span>**

| **<span dir="ltr">Method</span> <span dir="rtl">ונתיב</span>**<span dir="ltr"></span> | **<span dir="rtl">הרשאה</span>**<span dir="ltr"></span> | **<span dir="rtl">מטרה</span>**<span dir="ltr"></span> |
|----|----|----|
| **GET /api/v1/admins** | <span dir="rtl">מנהל</span><span dir="ltr"></span> | <span dir="rtl">רשימת פרופילי</span> <span dir="ltr">Admin</span> <span dir="rtl">ופרטי</span> <span dir="ltr">User</span> <span dir="rtl">מותרים.</span><span dir="ltr"></span> |
| **GET /api/v1/admins/{id}** | <span dir="rtl">מנהל</span><span dir="ltr"></span> | <span dir="rtl">פרופיל מנהל;</span> <span dir="ltr">id</span> <span dir="rtl">הוא</span> <span dir="ltr">Admin.Id.</span> |
| **GET /api/v1/admin/dashboard** | <span dir="rtl">מנהל</span><span dir="ltr"></span> | <span dir="rtl">מדדים לפי התרומות, הזכאות והמימושים.</span><span dir="ltr"></span> |
| **GET /api/v1/admin/audit** | <span dir="rtl">מנהל מורשה</span><span dir="ltr"></span> | <span dir="rtl">אירועי ביקורת ממקור הלוגים, אם הוגדר.</span><span dir="ltr"></span> |
| **<span dir="rtl">יצירת מנהל ושינוי תפקיד</span>**<span dir="ltr"></span> | <span dir="rtl">לא פתוח ב־</span><span dir="ltr">API</span> | <span dir="rtl">מסלול מבוקר יוגדר בנפרד; אין</span> <span dir="ltr">SuperAdmin</span> <span dir="rtl">או</span> <span dir="ltr">Role</span> <span dir="rtl">נוסף בטבלאות המקור.</span><span dir="ltr"></span> |

<span dir="ltr"></span>

## **<span dir="rtl">10.7 מבנה שגיאה</span>**

<span dir="rtl">כל שגיאה כוללת</span> <span dir="ltr">status, title, code</span> <span dir="rtl">עסקי יציב,</span> <span dir="ltr">detail</span> <span dir="rtl">בטוח להצגה, רשימת שגיאות שדות כאשר רלוונטי ו־</span><span dir="ltr">correlationId</span><span dir="rtl">. אין להחזיר הודעת</span> <span dir="ltr">DB</span> <span dir="rtl">גולמית או</span> <span dir="ltr">stack trace</span><span dir="rtl">.</span>

# **<span dir="rtl">11 ארכיטקטורת המערכת</span>**

## **<span dir="rtl">11.1 מבנה לוגי סופי</span>**

| **<span dir="rtl">שכבה</span>**<span dir="ltr"></span> | **<span dir="rtl">אחריות</span>**<span dir="ltr"></span> |
|----|----|
| <span dir="ltr">**Frontend**</span> | <span dir="rtl">יישום נפרד לתצוגה אינטראקציה ושמירת</span> <span dir="ltr">token</span> <span dir="rtl">בצד לקוח לפי מדיניות.</span><span dir="ltr"></span> |
| <span dir="ltr">**ASP.NET Core Web API**</span> | <span dir="rtl">נקודת כניסה יחידה לכל התפקידים והפעולות.</span><span dir="ltr"></span> |
| <span dir="ltr">**Controllers**</span> | <span dir="rtl">מיפוי</span> <span dir="ltr">HTTP</span> <span dir="rtl">אימות</span> <span dir="ltr">DTO</span> <span dir="rtl">בסיסי הפעלת</span> <span dir="ltr">Service</span> <span dir="rtl">והחזרת תגובה.</span><span dir="ltr"></span> |
| <span dir="ltr">**Services**</span> | <span dir="rtl">כללים עסקיים תהליכים מרובי טבלאות הרשאות עסקיות וטרנזקציות.</span><span dir="ltr"></span> |
| <span dir="ltr">**Repositories**</span> | <span dir="rtl">גישה לטבלה אחת לכל</span> <span dir="ltr">Repository</span> <span dir="rtl">ושאילתות הקשורות לאחריותה.</span><span dir="ltr"></span> |
| <span dir="ltr">**EF Core DbContext**</span> | <span dir="rtl">מיפוי</span> <span dir="ltr">Entities</span> <span dir="rtl">לסכמה הקיימת</span> <span dir="ltr">tracking</span> <span dir="rtl">ו־</span><span dir="ltr">transaction.</span> |
| <span dir="ltr">**MySQL**</span> | <span dir="rtl">מקור הנתונים הקיים שנוצר מראש על ידי סקריפט</span> <span dir="ltr">Python</span> <span dir="rtl">חיצוני.</span><span dir="ltr"></span> |

<span dir="ltr"></span>

<span dir="ltr"></span>

|  |
|----|
| **<span dir="rtl">זרימת תלות מחייבת:</span>**<span dir="ltr"> Frontend</span> <span dir="rtl">אל</span> <span dir="ltr">Controllers</span><span dir="rtl">, מהם אל</span> <span dir="ltr">Services</span><span dir="rtl">, מהם אל</span> <span dir="ltr">Repositories</span> <span dir="rtl">נפרדים, מהם אל</span> <span dir="ltr">EF Core DbContext</span> <span dir="rtl">ולבסוף אל</span> <span dir="ltr">MySQL</span><span dir="rtl">. אין דילוג על שכבה ואין שימוש ב־</span><span dir="ltr">DbContext</span> <span dir="rtl">מתוך</span> <span dir="ltr">Controller</span> <span dir="rtl">או</span> <span dir="ltr">Service.</span> |

<span dir="ltr"></span>

## **11.2 Repositories <span dir="rtl">נדרשים</span>**

| **Repository** | **<span dir="rtl">טבלה</span>**<span dir="ltr"></span> | **<span dir="rtl">אחריות</span>**<span dir="ltr"></span> |
|----|----|----|
| **IUserRepository** | User | <span dir="rtl">זהות, פרטי חשבון, גיבוב סיסמה, תפקיד ופעילות.</span><span dir="ltr"></span> |
| **IDonorRepository** | Donor | <span dir="rtl">פרופיל תורם וקישור</span> <span dir="ltr">UserId.</span> |
| **IRecipientRepository** | Recipient | <span dir="rtl">פרופיל סיוע, נתוני זכאות ועדכון יתרה מבוקר.</span><span dir="ltr"></span> |
| **IAdminRepository** | Admin | <span dir="rtl">פרופיל מנהל וקישור</span> <span dir="ltr">UserId.</span> |
| **IPopulationTypeRepository** | PopulationType | <span dir="rtl">מילון סוגי אוכלוסייה ופעילות.</span><span dir="ltr"></span> |
| **IDonationRepository** | Donation | <span dir="rtl">תרומות, סינון זמינות ועדכון יתרה מבוקר.</span><span dir="ltr"></span> |
| **IRedemptionRepository** | Redemption | <span dir="rtl">מימושים, סכומים מצטברים ומעברי מצב מבוקרים.</span><span dir="ltr"></span> |

<span dir="ltr"></span>

<span dir="rtl">כל ה־</span><span dir="ltr">Repositories</span> <span dir="rtl">בתהליך משתפים</span> <span dir="ltr">DbContext</span> <span dir="rtl">באותו</span> <span dir="ltr">scope</span><span dir="rtl">. ה־</span><span dir="ltr">Service</span> <span dir="rtl">מתאם ביניהם באמצעות חוזה יחידת עבודה לניהול</span> <span dir="ltr">commit</span> <span dir="rtl">ו־</span><span dir="ltr">rollback</span><span dir="rtl">, בלי לגשת ישירות ל־</span><span dir="ltr">DbContext</span><span dir="rtl">. רישום משתמש והשלמת מימוש הם תהליכים מרובי טבלאות;</span> <span dir="ltr">Repository</span> <span dir="rtl">נפרד לכל טבלה אינו משמעו טרנזקציה נפרדת לכל כתיבה.</span>

## **<span dir="rtl">11.3 מיפוי</span> EF Core <span dir="rtl">ללא שינוי</span> DB**

- <span dir="rtl">המיפוי מותאם לשמות הטבלאות העמודות המפתחות והיחסים שכבר קיימים.</span>

- <span dir="rtl">אין להריץ</span> <span dir="ltr">migrations add migrations update database</span> <span dir="rtl">או</span> <span dir="ltr">EnsureCreated</span><span dir="rtl">.</span>

- <span dir="rtl">אין הרשאות</span> <span dir="ltr">DDL</span> <span dir="rtl">לחשבון האפליקציה בסביבות בדיקה וייצור.</span>

- <span dir="rtl">בדיקת</span> <span dir="ltr">startup</span> <span dir="rtl">מאמתת חיבור וקריאת</span> <span dir="ltr">metadata</span> <span dir="rtl">או שאילתה בטוחה בלבד; היא אינה מתקנת סכימה.</span>

- <span dir="rtl">אי־התאמה בין</span> <span dir="ltr">Entities</span> <span dir="rtl">לסכמה גורמת לכשל ברור ומדווח, ולא לשינוי אוטומטי של</span> <span dir="ltr">DB</span><span dir="rtl">.</span>

- <span dir="ltr">Decimal precision, collation, enum mapping</span> <span dir="rtl">ותאריכים נקבעים בהתאם לסכמה הקיימת.</span>

## **<span dir="rtl">11.4 רכיבים רוחביים</span>**

| **<span dir="rtl">רכיב</span>**<span dir="ltr"></span> | **<span dir="rtl">אחריות</span>**<span dir="ltr"></span> |
|----|----|
| <span dir="ltr">**Authentication Authorization**</span> | <span dir="rtl">אימות</span> <span dir="ltr">token</span> <span dir="rtl">מדיניות תפקיד ובעלות.</span><span dir="ltr"></span> |
| <span dir="ltr">**Validation**</span> | <span dir="rtl">ולידציית</span> <span dir="ltr">DTO</span> <span dir="rtl">וכללים עסקיים בשירותים.</span><span dir="ltr"></span> |
| <span dir="ltr">**Exception Handling**</span> | <span dir="rtl">מיפוי חריגים ל־</span><span dir="ltr">Problem Details.</span> |
| <span dir="ltr">**Logging Audit**</span> | <span dir="rtl">לוג מובנה</span> <span dir="ltr">correlation</span> <span dir="rtl">ופעולות מנהל.</span><span dir="ltr"></span> |
| <span dir="ltr">**Mapping**</span> | <span dir="rtl">הפרדה בין</span> <span dir="ltr">DTOs</span> <span dir="rtl">ל־</span><span dir="ltr">Entities.</span> |
| <span dir="ltr">**Transactions**</span> | <span dir="rtl">אטומיות ברישום</span> <span dir="ltr">User</span> <span dir="rtl">ופרופיל ובהשלמת מימוש עם שתי היתרות.</span><span dir="ltr"></span> |
| <span dir="ltr">**OpenAPI**</span> | <span dir="rtl">תיעוד חוזה</span> <span dir="ltr">API</span> <span dir="rtl">ובדיקת התאמה מול</span> <span dir="ltr">Frontend.</span> |

<span dir="ltr"></span>

# **<span dir="rtl">12 דרישות לא פונקציונליות</span>**

| **<span dir="rtl">תחום</span>**<span dir="ltr"></span> | **<span dir="rtl">דרישה</span>**<span dir="ltr"></span> |
|----|----|
| **<span dir="rtl">ביצועים</span>**<span dir="ltr"></span> | <span dir="rtl">שאילתת רשימה רגילה עד 2 שניות ב־95% מהבקשות בתנאי עומס מוסכם; כל רשימה מדופדפת.</span><span dir="ltr"></span> |
| **<span dir="rtl">מקביליות</span>**<span dir="ltr"></span> | <span dir="rtl">אין חריגה מיתרות או ממגבלת נתרם גם במימושים מקבילים.</span><span dir="ltr"></span> |
| **<span dir="rtl">זמינות</span>**<span dir="ltr"></span> | <span dir="rtl">שגיאת</span> <span dir="ltr">DB</span> <span dir="rtl">מוחזרת באופן נשלט;</span> <span dir="ltr">health endpoint</span> <span dir="rtl">מבדיל בין</span> <span dir="ltr">liveness</span> <span dir="rtl">ל־</span><span dir="ltr">readiness.</span> |
| **<span dir="rtl">אבטחה</span>**<span dir="ltr"></span> | <span dir="ltr">HTTPS, least privilege</span><span dir="rtl">, סודות מחוץ לקוד,</span> <span dir="ltr">hash</span> <span dir="rtl">סיסמאות, הגבלת קצב למסלולי</span> <span dir="ltr">auth.</span> |
| **<span dir="rtl">פרטיות</span>**<span dir="ltr"></span> | <span dir="rtl">מינימום חשיפת נתונים; נתוני נתרם זמינים רק לבעלים ולמנהל מורשה.</span><span dir="ltr"></span> |
| **<span dir="rtl">תחזוקתיות</span>**<span dir="ltr"></span> | <span dir="rtl">שכבות מופרדות, חוזים ברורים,</span> <span dir="ltr">Repository</span> <span dir="rtl">לכל טבלה, בדיקות יחידה ואינטגרציה.</span><span dir="ltr"></span> |
| **<span dir="rtl">תצפיתיות</span>**<span dir="ltr"></span> | <span dir="rtl">לוגים מובנים עם</span> <span dir="ltr">userId role action entityId status duration correlationId.</span> |
| **<span dir="rtl">תאימות</span>**<span dir="ltr"></span> | <span dir="rtl">תיעוד</span> <span dir="ltr">OpenAPI</span> <span dir="rtl">גרסאי; שינוי שובר חוזה מחייב גרסה חדשה.</span><span dir="ltr"></span> |
| **<span dir="rtl">זמן</span>**<span dir="ltr"></span> | <span dir="rtl">מוסכמת</span> <span dir="ltr">UTC</span> <span dir="rtl">לכתיבות חדשות לאחר אימות מוסכמת הזמן בנתונים הקיימים; אין המרה גורפת של</span> <span dir="ltr">DB.</span> |
| **<span dir="rtl">נגישות</span> <span dir="ltr">Frontend</span>**<span dir="ltr"></span> | <span dir="rtl">מחוץ לפרויקט ה־</span><span dir="ltr">API</span> <span dir="rtl">אך חוזי השגיאה והסטטוסים צריכים לאפשר</span> <span dir="ltr">UI</span> <span dir="rtl">נגיש וברור.</span><span dir="ltr"></span> |

<span dir="ltr"></span>

# **<span dir="rtl">13 דיווח בקרה ותיעוד</span>**

- <span dir="rtl">הדוחות כוללים מספר תרומות, סכומים מקוריים ויתרות, סכום מימושים שהושלמו, יתרות זכאות ומימושים ממתינים. זמן טיפול ניתן לחשב מ־</span><span dir="ltr">CreatedAt</span> <span dir="rtl">ו־</span><span dir="ltr">RedeemedAt</span> <span dir="rtl">כאשר הם זמינים. אין מדד ״סכום מוקצה״ המבוסס על טבלה שאינה קיימת.</span>

- <span dir="rtl">דוחות מנהל מסננים לפי תקופה, סוג אוכלוסייה, סטטוס, תורם ונתרם בהתאם להרשאה. נתוני זהות נשלפים מ־</span><span dir="ltr">User</span> <span dir="rtl">ולא מועתקים לטבלאות התפקיד.</span>

- <span dir="rtl">אירועי ביקורת כוללים שינוי פעילות</span> <span dir="ltr">User</span><span dir="rtl">, שינוי פרטי זכאות, הפעלת או ביטול תרומה, השלמת או דחיית מימוש ושינוי סוג אוכלוסייה. נימוקי החלטות נשמרים בלוג המאובטח כשאין להם עמודה.</span>

- <span dir="rtl">כל אירוע כולל</span> <span dir="ltr">actor, action, entityType, entityId, timestamp, result, reason</span> <span dir="rtl">ו־</span><span dir="ltr">correlationId</span> <span dir="rtl">ככל שהתשתית מאפשרת.</span>

- <span dir="rtl">אם אין טבלת</span> <span dir="ltr">Audit</span> <span dir="rtl">בסכמת שבע הטבלאות, האירועים נשלחים לתשתית לוגים חיצונית או לקובץ לוג מאובטח; אין להוסיף טבלה ללא אישור ושינוי סקריפט ה־</span><span dir="ltr">DB</span> <span dir="rtl">החיצוני.</span>

# **<span dir="rtl">14 קריטריוני קבלה</span>**

| **<span dir="rtl">מזהה</span>**<span dir="ltr"></span> | **<span dir="rtl">קריטריון</span>**<span dir="ltr"></span> |
|----|----|
| **AC01** | <span dir="rtl">ה־</span><span dir="ltr">API</span> <span dir="rtl">מתחבר ל־</span><span dir="ltr">MySQL</span> <span dir="rtl">בלי ליצור או לשנות אובייקטי</span> <span dir="ltr">DB</span> <span dir="rtl">ובלי להריץ</span> <span dir="ltr">Python.</span> |
| **AC02** | <span dir="rtl">ממופות בדיוק שבע הטבלאות המקוריות ולכל אחת</span> <span dir="ltr">Repository</span> <span dir="rtl">נפרד.</span><span dir="ltr"></span> |
| **AC03** | <span dir="rtl">אין גישה ישירה מ־</span><span dir="ltr">Controller</span> <span dir="rtl">או</span> <span dir="ltr">Service</span> <span dir="rtl">ל־</span><span dir="ltr">DbContext.</span> |
| **AC04** | <span dir="rtl">רישום יוצר</span> <span dir="ltr">User</span> <span dir="rtl">ופרופיל תואם באופן אטומי; כשל לא משאיר חשבון יתום.</span><span dir="ltr"></span> |
| **AC05** | <span dir="ltr">Name, Phone</span> <span dir="rtl">ו־</span><span dir="ltr">PasswordHash</span> <span dir="rtl">נשמרים רק ב־</span><span dir="ltr">User; Donor</span> <span dir="rtl">ו־</span><span dir="ltr">Admin</span> <span dir="rtl">מכילות</span> <span dir="ltr">Id</span> <span dir="rtl">ו־</span><span dir="ltr">UserId</span> <span dir="rtl">בלבד.</span><span dir="ltr"></span> |
| **AC06** | <span dir="rtl">נתרם מעדכן שדות מותרים בלבד; אינו משנה זכאות, יתרות או תפקיד.</span><span dir="ltr"></span> |
| **AC07** | <span dir="rtl">התאמה דורשת אוכלוסייה זהה, פעילות, תוקף ויתרות תקינות.</span><span dir="ltr"></span> |
| **AC08** | <span dir="rtl">מימושים מקבילים אינם חורגים מיתרת תרומה, מיתרת זכאות או מהמגבלה המצטברת.</span><span dir="ltr"></span> |
| **AC09** | <span dir="rtl">השלמה חוזרת של אותו</span> <span dir="ltr">Redemption.Id</span> <span dir="rtl">אינה מפחיתה יתרות פעם נוספת.</span><span dir="ltr"></span> |
| **AC10** | <span dir="rtl">כשל במהלך השלמה מבטל את הסטטוס ושתי ההפחתות יחד.</span><span dir="ltr"></span> |
| **AC11** | <span dir="rtl">אין חישוב זכאות אוטומטי לפני אישור הנוסחה והמשקלים.</span><span dir="ltr"></span> |
| **AC12** | <span dir="rtl">ערכי</span> <span dir="ltr">UserType</span> <span dir="rtl">והסטטוסים ממופים לערכים הקיימים, בלי לתקן</span> <span dir="ltr">ENUM</span> <span dir="rtl">ב־</span><span dir="ltr">DB.</span> |
| **AC13** | <span dir="rtl">משתמש אינו קורא או משנה פרטי משתמש אחר ללא הרשאה; השבתת</span> <span dir="ltr">User</span> <span dir="rtl">נאכפת.</span><span dir="ltr"></span> |
| **AC14** | <span dir="ltr">OpenAPI</span> <span dir="rtl">מתאר את החוזים המעודכנים ללא נתיבי</span> <span dir="ltr">aid-requests</span> <span dir="rtl">או</span> <span dir="ltr">allocations.</span> |
| **AC15** | <span dir="rtl">שגיאות עקביות ופעולות מנהל מתועדות ללא חשיפת סיסמאות או מידע רגיש.</span><span dir="ltr"></span> |

<span dir="ltr"></span>

# **<span dir="rtl">15 החלטות פתוחות וסיכונים</span>**

| **<span dir="rtl">עדיפות</span>**<span dir="ltr"></span> | **<span dir="rtl">נושא</span>**<span dir="ltr"></span> | **<span dir="rtl">השפעה</span>**<span dir="ltr"></span> | **<span dir="rtl">אחראי להכרעה</span>**<span dir="ltr"></span> |
|----|----|----|----|
| **<span dir="rtl">קריטי</span>**<span dir="ltr"></span> | <span dir="rtl">משקלי ונוסחת זכאות</span><span dir="ltr"></span> | <span dir="rtl">אין חישוב אוטומטי; יש להגדיר גם תקופת הכנסה וזכאות.</span><span dir="ltr"></span> | <span dir="rtl">בעל מוצר</span><span dir="ltr"></span> |
| **<span dir="rtl">קריטי</span>**<span dir="ltr"></span> | <span dir="rtl">התאמה ל־</span><span dir="ltr">MySQL</span> <span dir="rtl">הקיים</span><span dir="ltr"></span> | <span dir="rtl">בדיקת שמות, טיפוסים,</span> <span dir="ltr">NULL, ENUM, FK</span> <span dir="rtl">וייחודיות</span> <span dir="ltr">UserId</span> <span dir="rtl">בקריאה בלבד.</span><span dir="ltr"></span> | <span dir="ltr">Backend</span> <span dir="rtl">ובעל</span> <span dir="ltr">DB</span> |
| **<span dir="rtl">גבוה</span>**<span dir="ltr"></span> | <span dir="rtl">משמעות סטטוס</span> <span dir="ltr">Donation</span> | <span dir="rtl">לאשר את</span> <span dir="ltr">PENDING</span> <span dir="rtl">כמצב פתוח ואת</span> <span dir="ltr">IsActive</span> <span dir="rtl">כשער אישור; אין לשנות</span> <span dir="ltr">ENUM.</span> | <span dir="rtl">בעל מוצר</span><span dir="ltr"></span> |
| **<span dir="rtl">גבוה</span>**<span dir="ltr"></span> | <span dir="rtl">מנהל ומועד במימוש ממתין</span><span dir="ltr"></span> | <span dir="rtl">יש לבדוק אם</span> <span dir="ltr">HandledByAdminId</span> <span dir="rtl">ו־</span><span dir="ltr">RedeemedAt</span> <span dir="rtl">מאפשרים</span> <span dir="ltr">NULL</span><span dir="rtl">; אחרת התהליך דורש הכרעה.</span><span dir="ltr"></span> | <span dir="ltr">Backend</span> <span dir="rtl">ובעל מוצר</span><span dir="ltr"></span> |
| **<span dir="rtl">גבוה</span>**<span dir="ltr"></span> | <span dir="rtl">מגבלה מצטברת לנתרם</span><span dir="ltr"></span> | <span dir="rtl">לאשר ש־</span><span dir="ltr">MaxAmountPerRecipient</span> <span dir="rtl">הוא סך כל המימושים של אותו נתרם מאותה תרומה.</span><span dir="ltr"></span> | <span dir="rtl">בעל מוצר</span><span dir="ltr"></span> |
| **<span dir="rtl">גבוה</span>**<span dir="ltr"></span> | <span dir="rtl">חישוב מחדש של זכאות</span><span dir="ltr"></span> | <span dir="rtl">למנוע איפוס יתרה או הענקת סיוע חוזר לא מאושרים.</span><span dir="ltr"></span> | <span dir="rtl">בעל מוצר</span><span dir="ltr"></span> |
| **<span dir="rtl">גבוה</span>**<span dir="ltr"></span> | <span dir="rtl">מניעת יצירת מימוש כפול</span><span dir="ltr"></span> | <span dir="rtl">אין עמודת מפתח בקשה; לבחור פתרון התמדה מותר ללא שינוי</span> <span dir="ltr">DB.</span> | <span dir="ltr">Backend</span> <span dir="rtl">ותשתיות</span><span dir="ltr"></span> |
| **<span dir="rtl">גבוה</span>**<span dir="ltr"></span> | <span dir="rtl">רישום וייחודיות חשבון</span><span dir="ltr"></span> | <span dir="rtl">להכריע</span> <span dir="ltr">Phone</span> <span dir="rtl">או</span> <span dir="ltr">User.Id</span> <span dir="rtl">להתחברות; לאמת אילוץ פרופיל יחיד ומענה למקביליות.</span><span dir="ltr"></span> | <span dir="ltr">Backend</span> <span dir="rtl">ואבטחה</span><span dir="ltr"></span> |
| **<span dir="rtl">בינוני</span>**<span dir="ltr"></span> | <span dir="rtl">הפעלת חשבונות והקמת מנהלים</span><span dir="ltr"></span> | <span dir="rtl">אין רישום מנהל ציבורי או תפקיד</span> <span dir="ltr">SuperAdmin</span><span dir="rtl">; נדרשת מדיניות מבוקרת.</span><span dir="ltr"></span> | <span dir="rtl">בעל מוצר ואבטחה</span><span dir="ltr"></span> |
| **<span dir="rtl">בינוני</span>**<span dir="ltr"></span> | <span dir="ltr">JWT</span> <span dir="rtl">וחידוש אסימון</span><span dir="ltr"></span> | <span dir="ltr">Refresh</span> <span dir="rtl">אינו חלק מחוזה הבסיס עד פתרון אחסון וביטול מאושר.</span><span dir="ltr"></span> | <span dir="rtl">אבטחה</span><span dir="ltr"></span> |
| **<span dir="rtl">בינוני</span>**<span dir="ltr"></span> | <span dir="rtl">מקור ביקורת ושחזור החלטות</span><span dir="ltr"></span> | <span dir="rtl">אין טבלת</span> <span dir="ltr">Audit</span> <span dir="rtl">או</span> <span dir="ltr">EligibilityVersion</span><span dir="rtl">; נדרשת תשתית לוגים מאובטחת.</span><span dir="ltr"></span> | <span dir="rtl">תשתיות</span><span dir="ltr"></span> |
| **<span dir="rtl">בינוני</span>**<span dir="ltr"></span> | <span dir="rtl">שדות נרטיביים חסרים</span><span dir="ltr"></span> | <span dir="rtl">מיקום ומצב משפחתי מוזכרים במקור ללא עמודות; אין לאסוף נתון שאין דרך מאושרת לשמור.</span><span dir="ltr"></span> | <span dir="rtl">בעל מוצר</span><span dir="ltr"></span> |

<span dir="ltr"></span>

## **<span dir="rtl">15.1 סיכונים מרכזיים ומענה</span>**

| **<span dir="rtl">סיכון</span>**<span dir="ltr"></span> | **<span dir="rtl">חומרה</span>**<span dir="ltr"></span> | **<span dir="rtl">מענה</span>**<span dir="ltr"></span> |
|----|----|----|
| **<span dir="rtl">פער בין התכנון לסכמה</span>**<span dir="ltr"></span> | <span dir="rtl">גבוהה</span><span dir="ltr"></span> | <span dir="rtl">מיפוי מפורש ובדיקה לקריאה בלבד; דיווח פער ולא תיקון אוטומטי.</span><span dir="ltr"></span> |
| **<span dir="rtl">כפילות זהות או פרופיל</span>**<span dir="ltr"></span> | <span dir="rtl">גבוהה</span><span dir="ltr"></span> | <span dir="ltr">User</span> <span dir="rtl">מרכזית, טרנזקציית רישום ובדיקת ייחודיות הנתמכת בסכמה.</span><span dir="ltr"></span> |
| **<span dir="rtl">משקלי זכאות לא מאושרים</span>**<span dir="ltr"></span> | <span dir="rtl">גבוהה</span><span dir="ltr"></span> | <span dir="rtl">חישוב אוטומטי חסום; מסלול ידני רק לאחר אישור מדיניות.</span><span dir="ltr"></span> |
| **<span dir="rtl">מימוש יתר במקביליות</span>**<span dir="ltr"></span> | <span dir="rtl">גבוהה</span><span dir="ltr"></span> | <span dir="rtl">טרנזקציה ובדיקת יתרות ומגבלה תחת בקרת מקביליות.</span><span dir="ltr"></span> |
| **<span dir="rtl">מימוש כפול</span>**<span dir="ltr"></span> | <span dir="rtl">גבוהה</span><span dir="ltr"></span> | <span dir="ltr">complete</span> <span dir="rtl">בטוח לחזרה; פער יצירה כפולה נשאר חסם עד בחירת פתרון.</span><span dir="ltr"></span> |
| **<span dir="rtl">חשיפת מידע אישי</span>**<span dir="ltr"></span> | <span dir="rtl">גבוהה</span><span dir="ltr"></span> | <span dir="ltr">DTO</span> <span dir="rtl">מצומצם,</span> <span dir="ltr">User.Id</span> <span dir="rtl">מאומת ובדיקת בעלות דרך פרופיל התפקיד.</span><span dir="ltr"></span> |
| **<span dir="rtl">פרשנות שגויה לסטטוס תרומה</span>**<span dir="ltr"></span> | <span dir="rtl">גבוהה</span><span dir="ltr"></span> | <span dir="rtl">אישור עסקי מפורש של משמעות ארבעת הערכים המקוריים.</span><span dir="ltr"></span> |
| **<span dir="rtl">שינוי</span> <span dir="ltr">DB</span> <span dir="rtl">בטעות</span>**<span dir="ltr"></span> | <span dir="rtl">גבוהה</span><span dir="ltr"></span> | <span dir="rtl">חשבון ללא</span> <span dir="ltr">DDL</span><span dir="rtl">; ללא</span> <span dir="ltr">migrations, EnsureCreated</span> <span dir="rtl">או</span> <span dir="ltr">scripts</span> <span dir="rtl">בפריסה.</span><span dir="ltr"></span> |

<span dir="ltr"></span>

# **<span dir="rtl">16 מילון מונחים</span>**

| **<span dir="rtl">מונח</span>**<span dir="ltr"></span> | **<span dir="rtl">הגדרה</span>**<span dir="ltr"></span> |
|----|----|
| **User** | <span dir="rtl">זהות החשבון המשותפת לכל התפקידים, כולל התחברות ופעילות.</span><span dir="ltr"></span> |
| **<span dir="rtl">פרופיל תפקיד</span>**<span dir="ltr"></span> | <span dir="rtl">רשומת</span> <span dir="ltr">Donor, Recipient</span> <span dir="rtl">או</span> <span dir="ltr">Admin</span> <span dir="rtl">המקושרת לחשבון באמצעות</span> <span dir="ltr">UserId.</span> |
| **PopulationType** | <span dir="rtl">מילון סוגי אוכלוסייה המשמש להתאמה בין תרומה לנתרם.</span><span dir="ltr"></span> |
| **Donation** | <span dir="rtl">תרומה בסכום לוגי, עם יתרה, תוקף ומגבלת סכום לכל נתרם.</span><span dir="ltr"></span> |
| **EligibilityAmount** | <span dir="rtl">סכום הזכאות שנקבע לנתרם; אינו ציון תעדוף.</span><span dir="ltr"></span> |
| **RemainingEligibility** | <span dir="rtl">יתרת סכום הזכאות שעוד ניתן לממש.</span><span dir="ltr"></span> |
| **Redemption** | <span dir="rtl">מימוש סכום מתרומה בידי נתרם, ללא ישות הקצאה ביניהם.</span><span dir="ltr"></span> |
| **Repository** | <span dir="rtl">רכיב גישה לטבלה אחת המבודד את השירותים מ־</span><span dir="ltr">EF Core.</span> |
| **<span dir="rtl">יחידת עבודה</span>**<span dir="ltr"></span> | <span dir="rtl">תיאום טרנזקציה בין מספר</span> <span dir="ltr">Repositories</span> <span dir="rtl">באותו</span> <span dir="ltr">DbContext.</span> |
| **Idempotency** | <span dir="rtl">חזרה על פעולה אינה מייצרת השפעה נוספת; יש להבחין בין השלמת מימוש ליצירתו.</span><span dir="ltr"></span> |

<span dir="ltr"></span>

# **<span dir="rtl">סיכום מחייב</span>**

<span dir="rtl">המודל כולל שבע טבלאות:</span> <span dir="ltr">User, Donor, Recipient, Admin, PopulationType, Donation</span> <span dir="rtl">ו־</span><span dir="ltr">Redemption</span><span dir="rtl">. זהות והתחברות נשמרות ב־</span><span dir="ltr">User</span> <span dir="rtl">בלבד, ופרטי התפקיד בטבלאות הרזות. הפיתוח נשאר</span> <span dir="ltr">API</span> <span dir="rtl">יחיד ב־</span><span dir="ltr">ASP.NET Core</span> <span dir="rtl">מול</span> <span dir="ltr">Frontend</span> <span dir="rtl">נפרד, עם</span> <span dir="ltr">Controllers, Services, Repository</span> <span dir="rtl">לכל טבלה,</span> <span dir="ltr">EF Core DbContext</span> <span dir="rtl">ו־</span><span dir="ltr">MySQL</span> <span dir="rtl">קיים. אין להקים או לשנות</span> <span dir="ltr">DB</span> <span dir="rtl">ואין לקבע אלגוריתם זכאות לפני אישור המשקלים והכללים.</span>
