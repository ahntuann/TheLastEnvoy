using UnityEngine;

public class GrassHideZone : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        Player01Controller player = other.GetComponent<Player01Controller>();
        SpriteRenderer sr = other.GetComponent<SpriteRenderer>();

        // Nếu đang tấn công => không thể ẩn nấp
        if (player != null && player.isAttacking)
        {
            if (sr != null)
                sr.sortingOrder = 4; // Player ở trước Grass
            player.isHiddenInGrass = false;
            return;
        }

        // Nếu không tấn công => ẩn nấp bình thường
        if (sr != null)
            sr.sortingOrder = 1; // Player nằm sau Grass
        if (player != null)
            player.isHiddenInGrass = true;
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        Player01Controller player = other.GetComponent<Player01Controller>();
        SpriteRenderer sr = other.GetComponent<SpriteRenderer>();

        // Nếu Player tấn công trong khi đang ở trong bụi => thoát khỏi ẩn nấp
        if (player != null && player.isAttacking)
        {
            if (sr != null)
                sr.sortingOrder = 4;
            player.isHiddenInGrass = false;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        Player01Controller player = other.GetComponent<Player01Controller>();
        SpriteRenderer sr = other.GetComponent<SpriteRenderer>();

        if (sr != null)
            sr.sortingOrder = 4; // Player trở lại bình thường
        if (player != null)
            player.isHiddenInGrass = false;
    }
}
