// ============================================================
// Navigation
// ============================================================
function showPage(page) {
    document.querySelectorAll('.page-section').forEach(el => el.classList.remove('active'));
    document.getElementById('page-' + page).classList.add('active');

    document.querySelectorAll('.nav-btn').forEach(btn => btn.classList.remove('active-page'));
    document.querySelector(`.nav-btn[data-page="${page}"]`)?.classList.add('active-page');

    // Load data when navigating to a page
    if (page === 'books') loadBooks();
    if (page === 'orders') loadOrders();
    if (page === 'place-order') loadBooksForDropdown();
}

document.querySelectorAll('.nav-btn').forEach(btn => {
    btn.addEventListener('click', function (e) {
        e.preventDefault();
        showPage(this.dataset.page);
    });
});

// ============================================================
// Books Page
// ============================================================
async function loadBooks() {
    try {
        const res = await fetch('/api/books');
        const books = await res.json();
        const grid = document.getElementById('books-grid');

        if (books.length === 0) {
            grid.innerHTML = '<p class="text-muted">No books found.</p>';
            return;
        }

        grid.innerHTML = books.map(book => `
            <div class="col-md-4 mb-3">
                <div class="card h-100">
                    <div class="card-body">
                        <h5 class="card-title">${esc(book.title)}</h5>
                        <h6 class="card-subtitle mb-2 text-muted">${esc(book.author)}</h6>
                        <span class="badge ${book.category === 'fiction' ? 'bg-primary' : 'bg-success'} mb-2">
                            ${esc(book.category)}
                        </span>
                        <p class="card-text">
                            <strong>Price:</strong> $${book.price.toFixed(2)}<br/>
                            <strong>Stock:</strong>
                            <span class="${book.stock < 10 ? 'text-danger fw-bold' : ''}">
                                ${book.stock} units
                            </span>
                        </p>
                    </div>
                </div>
            </div>
        `).join('');
    } catch (err) {
        console.error('Failed to load books:', err);
    }
}

// ============================================================
// Place Order Page
// ============================================================
async function loadBooksForDropdown() {
    try {
        const res = await fetch('/api/books');
        const books = await res.json();
        const select = document.getElementById('order-book');

        select.innerHTML = '<option value="0">-- Select a book --</option>';
        books.forEach(book => {
            const opt = document.createElement('option');
            opt.value = book.id;
            opt.textContent = `${book.title} ($${book.price.toFixed(2)}) - Stock: ${book.stock}`;
            select.appendChild(opt);
        });
    } catch (err) {
        console.error('Failed to load books for dropdown:', err);
    }
}

async function placeOrder() {
    const bookId = parseInt(document.getElementById('order-book').value);
    if (bookId === 0) {
        showOrderResult(true, 'Please select a book.');
        return;
    }

    const btn = document.getElementById('btn-place-order');
    btn.disabled = true;
    btn.textContent = 'Placing...';

    const order = {
        bookId: bookId,
        customerName: document.getElementById('order-name').value,
        customerEmail: document.getElementById('order-email').value,
        customerPhone: document.getElementById('order-phone').value,
        quantity: parseInt(document.getElementById('order-qty').value) || 1,
        notificationMethod: document.getElementById('order-notify').value
    };

    try {
        const res = await fetch('/api/orders', {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(order)
        });

        if (res.ok) {
            const result = await res.json();
            const text = `Order ID: ${result.id}\n`
                + `Total: $${result.totalPrice.toFixed(2)}\n`
                + `Discount: ${(result.discountApplied * 100).toFixed(0)}%\n`
                + `Tracking Note: ${result.trackingNote || 'NULL'}\n`
                + `Status: ${result.status}`;
            showOrderResult(false, text);
        } else {
            const errText = await res.text();
            showOrderResult(true, errText);
        }

        await loadBooksForDropdown();
    } catch (err) {
        showOrderResult(true, 'Network error: ' + err.message);
    }

    btn.disabled = false;
    btn.textContent = 'Place Order';
}

function showOrderResult(isError, message) {
    const div = document.getElementById('order-result');
    div.style.display = 'block';
    div.className = `alert ${isError ? 'alert-danger' : 'alert-success'}`;
    div.innerHTML = `
        <h5>${isError ? 'Error' : 'Order Placed!'}</h5>
        <pre style="white-space: pre-wrap; font-size: 0.85rem; margin: 0;">${esc(message)}</pre>
    `;
}

// ============================================================
// Orders Page
// ============================================================
async function loadOrders() {
    try {
        const res = await fetch('/api/orders');
        const orders = await res.json();
        const container = document.getElementById('orders-content');

        if (orders.length === 0) {
            container.innerHTML = '<p class="text-muted">No orders yet. Place one first!</p>';
            return;
        }

        container.innerHTML = `
            <table class="table table-striped">
                <thead>
                    <tr>
                        <th>ID</th>
                        <th>Customer</th>
                        <th>Book ID</th>
                        <th>Qty</th>
                        <th>Discount</th>
                        <th>Total</th>
                        <th>Notification</th>
                        <th>Tracking Note</th>
                        <th>Status</th>
                        <th>Actions</th>
                    </tr>
                </thead>
                <tbody>
                    ${orders.map(o => `
                        <tr class="${o.status === 'cancelled' ? 'table-secondary' : ''}">
                            <td>${o.id}</td>
                            <td>${esc(o.customerName)}</td>
                            <td>${o.bookId}</td>
                            <td>${o.quantity}</td>
                            <td>${(o.discountApplied * 100).toFixed(0)}%</td>
                            <td>$${o.totalPrice.toFixed(2)}</td>
                            <td><span class="badge bg-info">${esc(o.notificationMethod)}</span></td>
                            <td>${o.trackingNote ? esc(o.trackingNote) : '<span class="text-muted">N/A</span>'}</td>
                            <td>
                                <span class="badge ${o.status === 'confirmed' ? 'bg-success' : 'bg-secondary'}">
                                    ${esc(o.status)}
                                </span>
                            </td>
                            <td>
                                ${o.status === 'confirmed'
                                    ? `<button class="btn btn-sm btn-outline-danger" onclick="cancelOrder(${o.id})">Cancel</button>`
                                    : ''
                                }
                            </td>
                        </tr>
                    `).join('')}
                </tbody>
            </table>
        `;
    } catch (err) {
        console.error('Failed to load orders:', err);
    }
}

async function cancelOrder(id) {
    try {
        await fetch(`/api/orders/${id}/cancel`, { method: 'PATCH' });
        await loadOrders();
    } catch (err) {
        console.error('Failed to cancel order:', err);
    }
}

// ============================================================
// Utility
// ============================================================
function esc(str) {
    if (str == null) return '';
    const div = document.createElement('div');
    div.textContent = String(str);
    return div.innerHTML;
}

// ============================================================
// Initial load
// ============================================================
loadBooks();
