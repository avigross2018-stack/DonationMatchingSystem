# Backend gaps affecting the frontend

## Resolved capabilities

- Active population types are available through `GET /api/population-types` and are wired to recipient registration and donation creation.
- Administration APIs for users, donations, population types and dashboard statistics are available and wired to the admin UI.

## Deployment CORS — high

- Capability: allow the deployed frontend origin to call the API.
- Impact: separate-origin production deployment cannot call the API.
- Proposed change: allow only explicitly configured frontend origins, or deploy behind a same-origin reverse proxy.
- Frontend fallback: Vite proxy is used for local development.

## Donation detail and pagination — medium

- Missing: donor donation detail endpoint and server-side pagination.
- Impact: the frontend loads the current donation list and locates an item locally; direct edit routes require that list request.
- Proposed endpoints: `GET /api/donations/{id}` and paged `GET /api/donations`.

## Donor redemption visibility — medium

- Missing: redemptions created from the donor's donations.
- Impact: the donor dashboard cannot report recipients or matching history.

## Authentication lifecycle — documented limitation

- Missing: refresh token and logout endpoint.
- Frontend fallback: access token is held in memory and `sessionStorage`; logout clears local session and sensitive query caches.

## Error shape consistency — low

- Current responses can be Problem Details, ValidationProblemDetails, `{ message }`, or empty authentication errors.
- Frontend fallback: one central error normalizer handles all current forms.
