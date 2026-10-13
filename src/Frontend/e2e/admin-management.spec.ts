import { expect, test } from '@playwright/test';

function token(role: 'ADMIN' | 'RECIPIENT') {
  const payload = Buffer.from(JSON.stringify({ sub: '1', role, exp: Math.floor(Date.now() / 1000) + 3600 })).toString('base64url');
  return `header.${payload}.signature`;
}

test('admin dashboard uses server statistics', async ({ page }) => {
  await page.addInitScript((value) => sessionStorage.setItem('donation-matching-session', value), token('ADMIN'));
  await page.route('**/api/admin/me', (route) => route.fulfill({ status: 200, contentType: 'application/json', body: JSON.stringify({ adminId: 1, userId: 1, name: 'מנהלת', phone: '0500000000', isActive: true, createdAt: new Date().toISOString() }) }));
  await page.route('**/api/admin/dashboard/statistics', (route) => route.fulfill({ status: 200, contentType: 'application/json', body: JSON.stringify({ generatedAtUtc: new Date().toISOString(), users: { donorsCount: 3, recipientsCount: 4, activeUsersCount: 7, inactiveUsersCount: 0 }, donations: { totalCount: 2, totalDonationAmount: 10000, availableDonationCount: 1, availableDonationBalance: 6000, activeCount: 1, completedCount: 1, cancelledCount: 0, expiredCount: 0 }, redemptions: { totalCount: 1, pendingCount: 1, completedCount: 0, rejectedCount: 0, cancelledCount: 0, completedRedemptionAmount: 0 }, populationTypes: [] }) }));
  await page.route('**/api/admin/redemptions/pending**', (route) => route.fulfill({ status: 200, contentType: 'application/json', body: JSON.stringify({ items: [], page: 1, pageSize: 5, totalCount: 0, totalPages: 0 }) }));
  await page.goto('/admin');
  await expect(page.getByText('משתמשים פעילים')).toBeVisible();
  await expect(page.getByText(/10,000/)).toBeVisible();
});

test('admin can create a population type without hard-coded ids', async ({ page }) => {
  await page.addInitScript((value) => sessionStorage.setItem('donation-matching-session', value), token('ADMIN'));
  let rows: Array<Record<string, unknown>> = [];
  let submitted: Record<string, unknown> | undefined;
  await page.route('**/api/admin/population-types', async (route) => {
    if (route.request().method() === 'POST') {
      submitted = await route.request().postDataJSON();
      const created = { id: 91, ...submitted, isActive: true };
      rows = [created];
      return route.fulfill({ status: 201, contentType: 'application/json', body: JSON.stringify(created) });
    }
    return route.fulfill({ status: 200, contentType: 'application/json', body: JSON.stringify(rows) });
  });
  await page.goto('/admin/population-types');
  await page.getByRole('button', { name: 'סוג חדש' }).click();
  await page.getByLabel('שם').fill('משפחות');
  await page.getByLabel('תיאור').fill('סיוע למשפחות');
  await page.getByLabel('סכום זכאות בסיסי').fill('2500');
  await page.getByRole('button', { name: 'שמירה' }).click();
  await expect(page.getByRole('heading', { name: 'משפחות' })).toBeVisible();
  expect(submitted).toEqual({ name: 'משפחות', description: 'סיוע למשפחות', baseEligibilityAmount: 2500 });
});

test('role guard blocks a recipient from admin routes', async ({ page }) => {
  await page.addInitScript((value) => sessionStorage.setItem('donation-matching-session', value), token('RECIPIENT'));
  await page.goto('/admin');
  await expect(page).toHaveURL(/\/forbidden$/);
});
