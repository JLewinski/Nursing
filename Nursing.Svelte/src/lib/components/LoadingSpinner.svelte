<script lang="ts">
    export let size = '2rem';
</script>

<div 
    class="spinner"
    style="--size: {size}"
>
    <div class="inner"></div>
</div>

<style>
    .spinner {
        width: var(--size);
        height: var(--size);
        position: relative;
    }

    .inner {
        width: 100%;
        height: 100%;
        border: 3px solid transparent;
        border-top-color: var(--primary-color);
        border-radius: 50%;
        animation: spin 1s linear infinite;
    }

    @keyframes spin {
        0% { transform: rotate(0deg); }
        100% { transform: rotate(360deg); }
    }
</style>