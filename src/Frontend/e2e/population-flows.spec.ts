import { expect, test } from '@playwright/test';

const populations = [{ id: 42, name: 'משפחות', description: 'סיוע למשפחות' }];

test('recipient registration submits the selected server population id', async ({ page }) => {
  let requestBody: Record<string, unknown> | undefined;
  await page.route('**/api/population-types', (route) => route.fulfill({ status: 200, contentType: 'application/json', body: JSON.stringify(populations) }));
  await page.route('**/api/Auth/register/recipient', async (route) => { requestBody = await route.request().postDataJSON(); await route.fulfill({ status: 201, contentType: 'application/json', body: JSON.stringify({ message: 'ok', userId: 10 }) }); });
  await page.goto('/register/recipient');
  await page.getByLabel('שם מלא').fill('ישראל ישראלי');
  await page.getByLabel('מספר טלפון').fill('0501234567');
  await page.getByLabel('סוג אוכלוסייה').click();
  await page.getByRole('option', { name: /משפחות/ }).click();
  await page.getByLabel('הכנסה חודשית').fill('1000');
  await page.getByLabel('מספר ילדים').fill('2');
  await page.getByLabel('סיבת הבקשה').fill('בקשת סיוע');
  await page.getByLabel('סיסמה', { exact: true }).fill('password1');
  await page.getByLabel('אימות סיסמה').fill('password1');
  await page.getByRole('button', { name: 'יצירת חשבון נתרם' }).click();
  await expect(page).toHaveURL(/\/login$/);
  expect(requestBody?.populationTypeId).toBe(42);
});

test('donor creates a donation with a server population id', async ({ page }) => {
  const payload = Buffer.from(JSON.stringify({ sub: '7', role: 'DONOR', exp: Math.floor(Date.now() / 1000) + 3600 })).toString('base64url');
  await page.addInitScript((token) => sessionStorage.setItem('donation-matching-session', token), `header.${payload}.signature`);
  let requestBody: Record<string, unknown> | undefined;
  await page.route('**/api/population-types', (route) => route.fulfill({ status: 200, contentType: 'application/json', body: JSON.stringify(populations) }));
  await page.route('**/api/donations/show-donations', (route) => route.fulfill({ status: 200, contentType: 'application/json', body: '[]' }));
  await page.route('**/api/donations', async (route) => { if (route.request().method() !== 'POST') return route.continue(); requestBody = await route.request().postDataJSON(); await route.fulfill({ status: 201, contentType: 'application/json', body: JSON.stringify({ message: 'ok', donationId: 3 }) }); });
  await page.goto('/donor/donations/new');
  await page.getByLabel('סוג אוכלוסייה').click();
  await page.getByRole('option', { name: /משפחות/ }).click();
  await page.getByLabel('סכום כולל').fill('5000');
  await page.getByLabel('סכום מרבי לנתרם').fill('500');
  await page.getByLabel('תיאור').fill('תרומת בדיקה');
  const tomorrow = new Date(Date.now() + 86_400_000).toISOString().slice(0, 16);
  await page.getByLabel('תאריך תפוגה').fill(tomorrow);
  await page.getByRole('button', { name: 'שמירה' }).click();
  await expect(page).toHaveURL(/\/donor\/donations$/);
  expect(requestBody?.populationTypeId).toBe(42);
});
