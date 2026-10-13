export const featureFlags = {
  populationTypes: import.meta.env.VITE_POPULATION_TYPES_ENABLED === 'true',
  mocks: import.meta.env.DEV && import.meta.env.VITE_USE_MOCKS === 'true',
} as const;
