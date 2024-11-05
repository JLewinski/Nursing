<script lang="ts">
  let count = $state(1);
</script>

<button
  onclick={() => {
    count++
  }}>need hug right now {"🤗".repeat(count)}</button
>
