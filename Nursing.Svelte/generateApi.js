import fs from "node:fs";
import path from "node:path";
import { generateApi } from "swagger-typescript-api";

generateApi({
    name: "Nursing",
    url: "./api.json",
    input: path.resolve(process.cwd(), "./api.json"),
    output: path.resolve(process.cwd(), "./src/lib/swaggerApi/"),
    generateResponses: true,
    cleanOutput: true,
    primitiveTypeConstructs: (constructs) => ({
        ...constructs,
        string: {
            "date-time": "Date",
        },
    })
});
