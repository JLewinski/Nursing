import fs from 'node:fs';
import path from 'node:path';
import { generateApi } from 'swagger-typescript-api';

generateApi({
    name: 'Nursing',
    url: './api.json',
    input: path.resolve(process.cwd(), "./api.json"),
    output: path.resolve(process.cwd(), "./src/lib/swaggerApi/"),
    // generateResponses: true,
    hooks: {
        onParseSchema: (originalSchema, parsedSchema) => {
            if (originalSchema.type === 'string' && ['date-time', 'date'].indexOf(originalSchema.format) > -1) {
                parsedSchema.content = 'Date';
              }
              return parsedSchema;
        }
    }
});