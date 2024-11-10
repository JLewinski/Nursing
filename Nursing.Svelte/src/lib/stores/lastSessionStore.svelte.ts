interface lastSession {
    lastSide: "left" | "right" | null;
    lastStartTime: Date | null;
}

function createStore() {
    const data = (() => {
        const localData = localStorage.getItem("lastSessionStore");
        if (localData) {
            const parsedData = JSON.parse(localData) as { lastSide: "left" | "right" | null, lastStartTime: string };
            return {
                lastSide: parsedData.lastSide,
                lastStartTime: new Date(parsedData.lastStartTime),
            }
        }
        
        return {
            lastSide: null,
            lastStartTime: null,
        };
    })();

    let lastSide = $state(data.lastSide);
    let lastStartTime = $state(data.lastStartTime);

    // $effect(() => {
    //     localStorage.setItem("lastSessionStore", JSON.stringify({ lastSide: lastSide, lastStartTime: lastStartTime }));
    // });

    return { lastSide, lastStartTime };
}

export const lastSession = createStore();
