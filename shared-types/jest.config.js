module.exports = {
  preset: 'ts-jest',
  testEnvironment: 'node',
  roots: ['<rootDir>/tests', '<rootDir>/src'],
  testMatch: ['**/__tests__/**/*.ts', '**/?(*.)+(spec|test).ts'],
  collectCoverageFrom: [
    'src/**/*.ts',
    '!src/**/*.d.ts',
    '!src/**/index.ts',
    '!src/types/**/*.ts',
    '!src/constants/**/*.ts'
  ],
  coverageThreshold: {
    global: {
      branches: 55,
      functions: 75,
      lines: 75,
      statements: 75
    }
  }
};
