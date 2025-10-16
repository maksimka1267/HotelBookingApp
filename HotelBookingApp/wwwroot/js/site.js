// wwwroot/js/site.js
(function () {
    // Bootstrap tooltips init + reveal-on-scroll
    document.addEventListener("DOMContentLoaded", () => {
        if (window.bootstrap) {
            Array.prototype.slice.call(document.querySelectorAll('[data-bs-toggle="tooltip"]'))
                .forEach(el => new bootstrap.Tooltip(el));
        }

        const els = document.querySelectorAll(".reveal");
        if (els.length && "IntersectionObserver" in window) {
            const io = new IntersectionObserver(entries => {
                entries.forEach(e => {
                    if (e.isIntersecting) {
                        e.target.classList.add("in");
                        io.unobserve(e.target);
                    }
                });
            }, { threshold: 0.08 });
            els.forEach(el => io.observe(el));
        } else {
            // без IO просто показать
            document.querySelectorAll(".reveal").forEach(el => el.classList.add("in"));
        }
    });

    // Глобально доступный тост
    window.showToast = function (message, type = "info") {
        const wrap = document.createElement("div");
        wrap.className = "position-fixed top-0 end-0 p-3";
        wrap.style.zIndex = 1080;

        const bgClass = ({
            success: "text-bg-success",
            danger: "text-bg-danger",
            warning: "text-bg-warning",
            info: "text-bg-primary"
        })[type] || "text-bg-primary";

        wrap.innerHTML = `
      <div class="toast ${bgClass}" role="alert" aria-live="assertive" aria-atomic="true">
        <div class="d-flex">
          <div class="toast-body">${message}</div>
          <button type="button" class="btn-close btn-close-white me-2 m-auto" data-bs-dismiss="toast"></button>
        </div>
      </div>`;
        document.body.appendChild(wrap);

        if (window.bootstrap?.Toast) {
            const toastEl = wrap.querySelector(".toast");
            const t = new bootstrap.Toast(toastEl, { delay: 2400 });
            t.show();
            toastEl.addEventListener("hidden.bs.toast", () => wrap.remove());
        } else {
            // fallback без bootstrap
            setTimeout(() => wrap.remove(), 2400);
        }
    };
})();
