# Backend gaps affecting the frontend

## Active population types — critical

- Capability: list active population types.
- Impact: blocks real recipient registration, donation creation and reliable admin population filtering.
- Proposed endpoint: `GET /api/population-types/active`.
- Proposed response: `[{ "id": 1, "name": "Example" }]`.
- Authorization: public, because registration requires it.
- Frontend fallback: disable affected production submissions with a transparent message; never hard-code IDs.

## Deployment CORS — high

- Capability: allow the deployed frontend origin to call the API.
- Impact: separate-origin production deployment cannot call the API.
- Proposed change: allow only explicitly configured frontend origins, or deploy behind a same-origin reverse proxy.
- Frontend fallback: Vite proxy is used for local development.

## Administration APIs — medium

- Missing: user management, donation management, population-type management and system statistics.
- Impact: these navigation items are not shown as active features.
- Frontend fallback: dashboard uses only the existing redemptions and admin profile endpoints.

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
