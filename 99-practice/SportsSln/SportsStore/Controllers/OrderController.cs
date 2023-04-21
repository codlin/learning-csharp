using Microsoft.AspNetCore.Mvc;

using SportsStore.Models;

namespace SportsStore.Controllers;

public class OrderController : Controller {
    private IOrderRepository repository;
    private Cart cart;

    public OrderController(IOrderRepository repoService, Cart cartService) {
        repository = repoService;
        cart = cartService;
    }

    // 处理 HttpGet，用户点击 Checkout 时请求在这儿处理，返回 Checkout 视图，包含一个新生成的空订单
    public ViewResult Checkout() => View(new Order());

    // 处理 HttpPost，用户点击 Complete Order 时产生
    [HttpPost]
    public IActionResult Checkout(Order order) {
        if (cart.Lines.Count == 0) {
            ModelState.AddModelError("", "Sorry, your cart is empty");
        }
        if (ModelState.IsValid) {
            order.Lines = cart.Lines.ToArray();
            repository.SaveOrder(order);
            cart.Clear();
            return RedirectToPage("/Completed", new { orderId = order.OrderID });
        } else {
            return View();
        }
    }
}