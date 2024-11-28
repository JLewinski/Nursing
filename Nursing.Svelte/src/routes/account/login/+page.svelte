<script lang="ts">
    import { enhance } from "$app/forms";
    import type { ActionData } from "./$types";

    let { form }: { form: ActionData } = $props();
</script>
<div class="container">
    <h1>Login/Register</h1>
    <form method="post" action="?/login" use:enhance>
        <div class="form-group">
            <label for="username">Username</label>
            <input class="form-control" name="username" />
        </div>
        <div class="form-group mb-2">
            <label for="password">Password</label>
            <input class="form-control" type="password" name="password" />
        </div>
        <button class="btn btn-primary">Login</button>
        <button class="btn btn-secondary" formaction="?/register">Register</button>
    </form>
    <p style="color: red">{form?.message ?? ""}</p>
</div>
