import fs from "node:fs";
import path from "node:path";
import { generateApi, generateTemplates } from "swagger-typescript-api";

generateTemplates({
    cleanOutput: false,
    output: "./templates",
    httpClientType: "fetch",
    modular: true,
    silent: false,
    rewrite: false,
}).then(() => {
    return generateApi({
        name: "Nursing",
        url: "./api.json",
        input: path.resolve(process.cwd(), "./api.json"),
        output: path.resolve(process.cwd(), "./src/lib/api/"),
        generateApi: true,
        generateClient: true,
        generateResponses: true,
        generateRouteTypes: true,
        defaultResponseType: 'string',
        templates: path.resolve(process.cwd(), "./templates"),
        primitiveTypeConstructs: (constructs) => ({
            ...constructs,
            string: {
                "date-time": "Date",
            },
        }),
        modular: true,
    });
});
