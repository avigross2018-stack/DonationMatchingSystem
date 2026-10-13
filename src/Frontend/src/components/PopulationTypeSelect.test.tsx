import { QueryClient, QueryClientProvider } from '@tanstack/react-query';
import { render, screen } from '@testing-library/react';
import userEvent from '@testing-library/user-event';
import { afterEach, describe, expect, it, vi } from 'vitest';
import { populationTypesApi } from '../api/services';
import { PopulationTypeSelect } from './PopulationTypeSelect';

function renderSelect() {
  const queryClient = new QueryClient({ defaultOptions: { queries: { retry: false } } });
  return render(<QueryClientProvider client={queryClient}><PopulationTypeSelect value="" onChange={() => undefined} /></QueryClientProvider>);
}

afterEach(() => vi.restoreAllMocks());

describe('PopulationTypeSelect', () => {
  it('loads population types from the API', async () => {
    vi.spyOn(populationTypesApi, 'listActive').mockResolvedValue([{ id: 7, name: 'משפחות', description: 'סיוע למשפחות' }]);
    renderSelect();
    const user = userEvent.setup();
    await user.click(await screen.findByRole('combobox'));
    expect(await screen.findByRole('option', { name: /משפחות/ })).toBeVisible();
  });

  it('shows a blocking empty state', async () => {
    vi.spyOn(populationTypesApi, 'listActive').mockResolvedValue([]);
    renderSelect();
    expect(await screen.findByText(/אין כרגע סוגי אוכלוסייה פעילים/)).toBeVisible();
  });

  it('shows a retry action when loading fails', async () => {
    vi.spyOn(populationTypesApi, 'listActive').mockRejectedValue(new Error('offline'));
    renderSelect();
    expect(await screen.findByText(/לא ניתן לטעון את סוגי האוכלוסייה/)).toBeVisible();
    expect(screen.getByRole('button', { name: 'נסו שוב' })).toBeEnabled();
  });
});
