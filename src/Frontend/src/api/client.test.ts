import axios, { AxiosHeaders } from 'axios';
import { describe, expect, it } from 'vitest';
import { normalizeError } from './client';
describe('normalizeError', () => {
  it('normalizes problem details', () => { const error = new axios.AxiosError('bad', 'ERR_BAD_RESPONSE', undefined, undefined, { status: 409, statusText: 'Conflict', headers: {}, config: { headers: new AxiosHeaders() }, data: { title: 'Conflict', detail: 'Changed', traceId: 'abc' } }); const result = normalizeError(error); expect(result.kind).toBe('conflict'); expect(result.message).toBe('Changed'); expect(result.traceId).toBe('abc'); });
  it('normalizes network errors', () => { const result = normalizeError(new axios.AxiosError('offline')); expect(result.kind).toBe('network'); });
});
