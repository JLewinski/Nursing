import { assertEquals } from "jsr:@std/assert";
import { migrateData, parseTimeSpan } from "./migration_old.ts";

const feeding = {
    Id: "d44a89a8-578d-4d05-a1bb-6c96b17cb9c1",
    LeftBreastTotal: "0.00:00:01:59",
    RightBreastTotal: "0.00:00:01:486",
    TotalTime: "0.00:00:03:076",
    Started: "2024-11-18T20:28:05.03Z",
    Finished: "2024-11-18T20:28:08.106Z",
    LastIsLeft: false,
    LastUpdated: "2024-11-18T20:28:08.12Z",
    Deleted: null,
};

Deno.test("migrateData", () => {
    const session = migrateData(feeding);
    assertEquals(session.id, "d44a89a8-578d-4d05-a1bb-6c96b17cb9c1");
    assertEquals(session.startTime, new Date("2024-11-18T20:28:05.03Z"));
    assertEquals(session.duration, 3076); // 3.076 seconds in milliseconds
    assertEquals(session.rightDuration, 1486); // 1.486 seconds in milliseconds
    assertEquals(session.leftDuration, 1590); // 1.59 seconds in milliseconds
    assertEquals(session.lastUpdated, new Date("2024-11-18T20:28:08.12Z"));
});

Deno.test("parseTimeSpan", () => {
    assertEquals(parseTimeSpan("0.00:00:01:9"), 1900); // 1.59 seconds in milliseconds
    assertEquals(parseTimeSpan("0.00:00:01:59"), 1590); // 1.59 seconds in milliseconds
    assertEquals(parseTimeSpan("0.00:00:01:486"), 1486); // 1.486 seconds in milliseconds
    assertEquals(parseTimeSpan("0.00:00:03:076"), 3076); // 3.076 seconds in milliseconds
});
