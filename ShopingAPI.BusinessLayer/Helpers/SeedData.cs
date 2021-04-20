using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using ShopingAPI.DataLayer.Models;
using ShopingAPI.DataLayer.Data;
using ShopingAPI.BusinessLayer.Repository;

namespace ShopingAPI.BusinessLayer.Helpers
{
    public class SeedData
    {
       
        public static void SeedDatas( DataContext context, UserManager<User> userManager,RoleManager<Role> roleManager, ISingletonRepository _singleton)
        {
            string userId=string.Empty;
            string userId2=string.Empty;
            //DynamicTableName tableNameShop = new DynamicTableName
            //{
            //    Id = "1EAF407F-4EF3-45BD-9864-E521C6A61831",
            //    TableName = "1EAF407F-4EF3-45BD-9864-E521C6A61831".Replace("-", ""),
            //    TableType="VO"
            //};
            DynamicTableName tableNameUser = new DynamicTableName
            {
                Id = "671576C9-CA08-4696-875A-24EFEA45004C",
                TableName = "671576C9-CA08-4696-875A-24EFEA45004C".Replace("-", ""),
                TableType = "UO"
            };

            if (!roleManager.Roles.Any())
            {
                var roles = new List<Role>()
                {
                    new Role{Name="User" },
                    new Role{Name="Vendor" },
                    new Role{Name="Admin" },
                    new Role{Name="VendorEmployee" }
                };
                foreach (var role in roles)
                {
                    role.Id = Guid.NewGuid().ToString();
                    roleManager.CreateAsync(role).Wait();
                }
                // Create Admin

                var adminUser1 = new User
                {
                    Id = Guid.NewGuid().ToString(),
                    FullName = "Pankaj",
                    UserName = "pan7753@gmail.com",
                    TableName=tableNameUser.TableName
                };
                var adminUser2 = new User
                {
                    Id = Guid.NewGuid().ToString(),
                    FullName = "Ravi",
                    UserName = "ravi@gmail.com",
                    TableName = tableNameUser.TableName
                };
                userId = adminUser1.Id;
                userId2 = adminUser2.Id;
                var result = userManager.CreateAsync(adminUser1, "password").Result;
                var result2 = userManager.CreateAsync(adminUser2, "password").Result;

                if (result.Succeeded|| result2.Succeeded)
                {
                    var admin1 = userManager.FindByNameAsync(adminUser1.UserName).Result;
                    var admin2 = userManager.FindByNameAsync(adminUser2.UserName).Result;
                    var result11 = userManager.AddToRolesAsync(admin1, new[] { "User",  "Admin", "VendorEmployee" }).Result;
                    var result22 = userManager.AddToRolesAsync(admin2, new[] { "User" }).Result;
                    var tableCreated =  _singleton.userRepository.CreateUserOrdersTable(admin2.TableName).Result;

                }
            }
            if (!context.Shops.Any())
            {
                DateTime dateTime = DateTime.UtcNow;
                string ipAddress = "";

                if (Dns.GetHostAddresses(Dns.GetHostName()).Length > 0)
                {
                    ipAddress = Dns.GetHostAddresses(Dns.GetHostName())[0].ToString();
                }
                var brands = new List<Brand>
                {
                    new Brand
                    {
                        Id="EB99263C-4A45-4D63-A36A-9C1A1CD382D7",
                        BrandName="Brand A"
                    },
                    new Brand
                    {
                        Id="E9BA24F6-89C7-4CE5-8004-A8A3D7D76A0A",
                        BrandName="Brand B"
                    },
                    new Brand
                    {
                        Id="E072A4F2-9854-488F-AAD3-F0510824FD02",
                        BrandName="Brand C"
                    }
                };
                var categories = new List<Category>
                {
                    new Category
                    {
                        Id="2ED11545-8EA4-45A4-A36E-364330636395",
                        Name="Vegetables",
                        IsActive=true,
                        CreatedDateTime=dateTime,
                        UpdatedDateTime=dateTime,
                        ModifyBy=userId,
                        IP=ipAddress,
                        ImageUrl=null,
                        ShortDescription=null,
                        SubCategories=new List<SubCategory>
                        {
                            new SubCategory
                            {
                                Id="037BC2D6-152A-48EB-8EAD-C64DC0337E77",
                                SubCategoryName="Potato",
                                IsActive=true,
                                CreatedDateTime=dateTime,
                                UpdatedDateTime=dateTime,
                                ModifyBy=userId,
                                IP=ipAddress,
                                Products=new List<Product>
                                {
                                    new Product
                                    {
                                        Id="ED8243A7-D696-4817-B0AF-D0A6D229C3FQ",
                                        ProductName="White Potato",
                                        IsActive=true,
                                        CreatedDateTime=dateTime,
                                        UpdatedDateTime=dateTime,
                                        ModifyBy=userId,
                                        IP=ipAddress,
                                        BrandId="EB99263C-4A45-4D63-A36A-9C1A1CD382D7",
                                        ImageUrl=null,
                                        CategoryId="2ED11545-8EA4-45A4-A36E-364330636395",
                                        Description=null
                                    },
                                    new Product
                                    {
                                        Id="2FE1B147-F14C-4857-88E6-AE19FD67EBFA",
                                        ProductName="Red Potato",
                                        IsActive=true,
                                        CreatedDateTime=dateTime,
                                        UpdatedDateTime=dateTime,
                                        ModifyBy=userId,
                                        IP=ipAddress,
                                        BrandId="EB99263C-4A45-4D63-A36A-9C1A1CD382D7",
                                        ImageUrl=null,
                                        CategoryId="2ED11545-8EA4-45A4-A36E-364330636395",
                                        Description=null
                                    }
                                }
                            }
                        },
                        Products=new List<Product>
                        {
                            new Product
                                    {
                                        Id="2C351CFC-D537-43A9-8B42-E9E7933208CD",
                                        ProductName="Pumpkin",
                                        IsActive=true,
                                        CreatedDateTime=dateTime,
                                        UpdatedDateTime=dateTime,
                                        ModifyBy=userId,
                                        BrandId="EB99263C-4A45-4D63-A36A-9C1A1CD382D7",
                                        IP=ipAddress,
                                        ImageUrl=null,
                                        Description=null
                                    },
                                    new Product
                                    {
                                        Id="DA25903D-C84F-410C-AD65-700DE95D3581",
                                        ProductName="Tomato",
                                        IsActive=true,
                                        CreatedDateTime=dateTime,
                                        UpdatedDateTime=dateTime,
                                        ModifyBy=userId,
                                        BrandId="EB99263C-4A45-4D63-A36A-9C1A1CD382D7",
                                        IP=ipAddress,
                                        ImageUrl=null,
                                        Description=null
                                    },
                                    new Product
                                    {
                                        Id="DA25903D-C84F-410C-AD65-700DE95D3582",
                                        ProductName="Broccoli",
                                        IsActive=true,
                                        CreatedDateTime=dateTime,
                                        UpdatedDateTime=dateTime,
                                        BrandId="E9BA24F6-89C7-4CE5-8004-A8A3D7D76A0A",
                                        ModifyBy=userId,
                                        IP=ipAddress,
                                        ImageUrl=null,
                                        Description=null
                                    }
                        }
                        
                    },
                    new Category
                    {
                        Id="632C0BC6-B420-4EF6-835D-7C1FBA2BE3EF",
                        Name="Grocery",
                        IsActive=true,
                        CreatedDateTime=dateTime,
                        UpdatedDateTime=dateTime,
                        ModifyBy=userId,
                        IP=ipAddress,
                        ImageUrl=null,
                        ShortDescription=null,
                        SubCategories=new List<SubCategory>
                        {
                            new SubCategory
                            {
                                Id="AF064AD7-6C70-4085-BE05-190387B20F9D",
                                SubCategoryName="SNACKS",
                                IsActive=true,
                                CreatedDateTime=dateTime,
                                UpdatedDateTime=dateTime,
                                ModifyBy=userId,
                                IP=ipAddress,
                                Products=new List<Product>
                                {
                                    new Product
                                    {
                                        Id="F05F5933-1108-4791-B4B0-061E86F78B9A",
                                        ProductName="Candy",
                                        IsActive=true,
                                        CreatedDateTime=dateTime,
                                        UpdatedDateTime=dateTime,
                                        ModifyBy=userId,
                                        BrandId="E9BA24F6-89C7-4CE5-8004-A8A3D7D76A0A",
                                        IP=ipAddress,
                                        ImageUrl=null,
                                        CategoryId="632C0BC6-B420-4EF6-835D-7C1FBA2BE3EF",
                                        Description=null
                                    },
                                    new Product
                                    {
                                        Id="EF03110E-6EA7-4890-8CFB-A3BC0831261D",
                                        ProductName="Cookies",
                                        IsActive=true,
                                        CreatedDateTime=dateTime,
                                        UpdatedDateTime=dateTime,
                                        ModifyBy=userId,
                                        BrandId="E9BA24F6-89C7-4CE5-8004-A8A3D7D76A0A",
                                        IP=ipAddress,
                                        ImageUrl=null,
                                        CategoryId="632C0BC6-B420-4EF6-835D-7C1FBA2BE3EF",
                                        Description=null
                                    },
                                    new Product
                                    {
                                        Id="F67D9790-B911-4F33-AEBC-E7165D8B0A5A",
                                        ProductName="Crackers",
                                        IsActive=true,
                                        CreatedDateTime=dateTime,
                                        BrandId="E9BA24F6-89C7-4CE5-8004-A8A3D7D76A0A",
                                        UpdatedDateTime=dateTime,
                                        ModifyBy=userId,
                                        IP=ipAddress,
                                        ImageUrl=null,
                                        CategoryId="632C0BC6-B420-4EF6-835D-7C1FBA2BE3EF",
                                        Description=null
                                    },
                                    new Product
                                    {
                                        Id="F66CA415-8D35-4F7D-AEDC-52BD963B5473",
                                        ProductName="Dried Fruits",
                                        IsActive=true,
                                        CreatedDateTime=dateTime,
                                        UpdatedDateTime=dateTime,
                                        BrandId="E9BA24F6-89C7-4CE5-8004-A8A3D7D76A0A",
                                        ModifyBy=userId,
                                        IP=ipAddress,
                                        ImageUrl=null,
                                        CategoryId="632C0BC6-B420-4EF6-835D-7C1FBA2BE3EF",
                                        Description=null
                                    },
                                    new Product
                                    {
                                        Id="318561D6-4F55-4FF4-8558-57C6973DA1F2",
                                        ProductName="Granola Bars",
                                        IsActive=true,
                                        CreatedDateTime=dateTime,
                                        UpdatedDateTime=dateTime,
                                        BrandId="E9BA24F6-89C7-4CE5-8004-A8A3D7D76A0A",
                                        ModifyBy=userId,
                                        IP=ipAddress,
                                        ImageUrl=null,
                                        CategoryId="632C0BC6-B420-4EF6-835D-7C1FBA2BE3EF",
                                        Description=null
                                    }
                                }
                            },
                            new SubCategory
                            {
                                Id="AF6690E4-9DA3-4C03-9EC6-626200FEAFD2",
                                SubCategoryName="OILS",
                                IsActive=true,
                                CreatedDateTime=dateTime,
                                UpdatedDateTime=dateTime,
                                ModifyBy=userId,
                                IP=ipAddress,
                                Products=new List<Product>
                                {
                                    new Product
                                    {
                                        Id="7C50D70D-B552-4BFE-AB7B-8F2A24547297",
                                        ProductName="Vegetable Oil",
                                        IsActive=true,
                                        CreatedDateTime=dateTime,
                                        UpdatedDateTime=dateTime,
                                        ModifyBy=userId,
                                        BrandId="E072A4F2-9854-488F-AAD3-F0510824FD02",
                                        IP=ipAddress,
                                        ImageUrl=null,
                                        CategoryId="632C0BC6-B420-4EF6-835D-7C1FBA2BE3EF",
                                        Description=null
                                    },
                                    new Product
                                    {
                                        Id="940A88E1-8D3F-4EDB-8AA2-09975250295A",
                                        ProductName="Soy Sauce",
                                        IsActive=true,
                                        CreatedDateTime=dateTime,
                                        UpdatedDateTime=dateTime,
                                        ModifyBy=userId,
                                        BrandId="E072A4F2-9854-488F-AAD3-F0510824FD02",
                                        IP=ipAddress,
                                        ImageUrl=null,
                                        CategoryId="632C0BC6-B420-4EF6-835D-7C1FBA2BE3EF",
                                        Description=null
                                    },
                                    new Product
                                    {
                                        Id="F5E0DFE2-9D31-43AD-988F-5690CE6AC956",
                                        ProductName="Olive Oil",
                                        IsActive=true,
                                        CreatedDateTime=dateTime,
                                        UpdatedDateTime=dateTime,
                                        ModifyBy=userId,
                                        BrandId="E072A4F2-9854-488F-AAD3-F0510824FD02",
                                        IP=ipAddress,
                                        ImageUrl=null,
                                        CategoryId="632C0BC6-B420-4EF6-835D-7C1FBA2BE3EF",
                                        Description=null
                                    },
                                    new Product
                                    {
                                        Id="233E962D-80B2-4370-BE8C-28CAEACF3E37",
                                        ProductName="Vinegar",
                                        IsActive=true,
                                        CreatedDateTime=dateTime,
                                        UpdatedDateTime=dateTime,
                                        ModifyBy=userId,
                                        BrandId="E072A4F2-9854-488F-AAD3-F0510824FD02",
                                        IP=ipAddress,
                                        ImageUrl=null,
                                        CategoryId="632C0BC6-B420-4EF6-835D-7C1FBA2BE3EF",
                                        Description=null
                                    },
                                    new Product
                                    {
                                        Id="0ED84D8F-B0C0-402C-B3E4-83150B6EBE3C",
                                        ProductName="BBQ Sauce",
                                        IsActive=true,
                                        CreatedDateTime=dateTime,
                                        UpdatedDateTime=dateTime,
                                        ModifyBy=userId,
                                        BrandId="E072A4F2-9854-488F-AAD3-F0510824FD02",
                                        IP=ipAddress,
                                        ImageUrl=null,
                                        CategoryId="632C0BC6-B420-4EF6-835D-7C1FBA2BE3EF",
                                        Description=null
                                    }
                                }
                            },
                            new SubCategory
                            {
                                Id="1B673BA2-F706-427D-A93B-FF21C667831E",
                                SubCategoryName="PERSONAL",
                                IsActive=true,
                                CreatedDateTime=dateTime,
                                UpdatedDateTime=dateTime,
                                ModifyBy=userId,
                                IP=ipAddress,
                                Products=new List<Product>
                                {
                                    new Product
                                    {
                                        Id="EF89EA74-A2EF-4E95-B2F5-F0FC57DD8551",
                                        ProductName="Conditioner",
                                        IsActive=true,
                                        CreatedDateTime=dateTime,
                                        UpdatedDateTime=dateTime,
                                        ModifyBy=userId,
                                        BrandId="E072A4F2-9854-488F-AAD3-F0510824FD02",
                                        IP=ipAddress,
                                        ImageUrl=null,
                                        CategoryId="632C0BC6-B420-4EF6-835D-7C1FBA2BE3EF",
                                        Description=null
                                    },
                                    new Product
                                    {
                                        Id="40D47283-994A-46B1-8D65-A0F9F6AAD4FF",
                                        ProductName="Cotton Products",
                                        IsActive=true,
                                        CreatedDateTime=dateTime,
                                        UpdatedDateTime=dateTime,
                                        ModifyBy=userId,
                                        BrandId="E072A4F2-9854-488F-AAD3-F0510824FD02",
                                        IP=ipAddress,
                                        ImageUrl=null,
                                        CategoryId="632C0BC6-B420-4EF6-835D-7C1FBA2BE3EF",
                                        Description=null
                                    },
                                    new Product
                                    {
                                        Id="AD4E5FD4-EF34-4876-A299-FBE894939664",
                                        ProductName="Hair Spray",
                                        IsActive=true,
                                        CreatedDateTime=dateTime,
                                        UpdatedDateTime=dateTime,
                                        ModifyBy=userId,
                                        BrandId="E072A4F2-9854-488F-AAD3-F0510824FD02",
                                        IP=ipAddress,
                                        ImageUrl=null,
                                        CategoryId="632C0BC6-B420-4EF6-835D-7C1FBA2BE3EF",
                                        Description=null
                                    },
                                    new Product
                                    {
                                        Id="7A9236B4-9850-43B1-A936-3300E6AB2E6F",
                                        ProductName="Soap",
                                        IsActive=true,
                                        CreatedDateTime=dateTime,
                                        UpdatedDateTime=dateTime,
                                        ModifyBy=userId,
                                        BrandId="E072A4F2-9854-488F-AAD3-F0510824FD02",
                                        IP=ipAddress,
                                        ImageUrl=null,
                                        CategoryId="632C0BC6-B420-4EF6-835D-7C1FBA2BE3EF",
                                        Description=null
                                    },
                                    new Product
                                    {
                                        Id="7CC74FFF-6D2A-4020-A1F9-B9881D70CA7A",
                                        ProductName="Shaving Cream",
                                        IsActive=true,
                                        CreatedDateTime=dateTime,
                                        UpdatedDateTime=dateTime,
                                        ModifyBy=userId,
                                        BrandId="E072A4F2-9854-488F-AAD3-F0510824FD02",
                                        IP=ipAddress,
                                        ImageUrl=null,
                                        CategoryId="632C0BC6-B420-4EF6-835D-7C1FBA2BE3EF",
                                        Description=null
                                    }
                                }
                            },
                            new SubCategory
                            {
                                Id="BC191FDD-5A5E-4A19-95F9-1DCC81095EFF",
                                SubCategoryName="DAIRY",
                                IsActive=true,
                                CreatedDateTime=dateTime,
                                UpdatedDateTime=dateTime,
                                ModifyBy=userId,
                                IP=ipAddress,
                                Products=new List<Product>
                                {
                                    new Product
                                    {
                                        Id="FD49177C-88B2-4E7C-8949-1F28CFA8E2DF",
                                        ProductName="Biscuits",
                                        IsActive=true,
                                        CreatedDateTime=dateTime,
                                        UpdatedDateTime=dateTime,
                                        ModifyBy=userId,
                                        BrandId="E072A4F2-9854-488F-AAD3-F0510824FD02",
                                        IP=ipAddress,
                                        ImageUrl=null,
                                        CategoryId="632C0BC6-B420-4EF6-835D-7C1FBA2BE3EF",
                                        Description=null
                                    },
                                    new Product
                                    {
                                        Id="3372D9B6-6BCD-44AB-9250-4DDB58431F71",
                                        ProductName="Butter",
                                        IsActive=true,
                                        CreatedDateTime=dateTime,
                                        UpdatedDateTime=dateTime,
                                        BrandId="E072A4F2-9854-488F-AAD3-F0510824FD02",
                                        ModifyBy=userId,
                                        IP=ipAddress,
                                        ImageUrl=null,
                                        CategoryId="632C0BC6-B420-4EF6-835D-7C1FBA2BE3EF",
                                        Description=null
                                    },
                                    new Product
                                    {
                                        Id="CAF722B0-5DF5-4F5F-B850-14988E3AB81C",
                                        ProductName="Cheese",
                                        IsActive=true,
                                        CreatedDateTime=dateTime,
                                        UpdatedDateTime=dateTime,
                                        ModifyBy=userId,
                                        BrandId="E072A4F2-9854-488F-AAD3-F0510824FD02",
                                        IP=ipAddress,
                                        ImageUrl=null,
                                        CategoryId="632C0BC6-B420-4EF6-835D-7C1FBA2BE3EF",
                                        Description=null
                                    },
                                    new Product
                                    {
                                        Id="1E0187D2-972E-4F0A-ADDE-182B6DB7ACBB",
                                        ProductName="Cookie Dough",
                                        IsActive=true,
                                        CreatedDateTime=dateTime,
                                        UpdatedDateTime=dateTime,
                                        ModifyBy=userId,
                                        BrandId="E072A4F2-9854-488F-AAD3-F0510824FD02",
                                        IP=ipAddress,
                                        ImageUrl=null,
                                        CategoryId="632C0BC6-B420-4EF6-835D-7C1FBA2BE3EF",
                                        Description=null
                                    },
                                    new Product
                                    {
                                        Id="AF9E329F-D3A1-4CDE-8B6A-8308A185E332",
                                        ProductName="Cream Cheese",
                                        IsActive=true,
                                        CreatedDateTime=dateTime,
                                        UpdatedDateTime=dateTime,
                                        ModifyBy=userId,
                                        BrandId="E072A4F2-9854-488F-AAD3-F0510824FD02",
                                        IP=ipAddress,
                                        ImageUrl=null,
                                        CategoryId="632C0BC6-B420-4EF6-835D-7C1FBA2BE3EF",
                                        Description=null
                                    }
                                }
                            }

                        }
                    },
                    new Category
                    {
                        Id="94661C67-9D3C-45F5-A6DE-50A28B3B1FF6",
                        Name="Electronic",
                        IsActive=true,
                        CreatedDateTime=dateTime,
                        UpdatedDateTime=dateTime,
                        ModifyBy=userId,
                        IP=ipAddress,
                        ImageUrl=null,
                        ShortDescription=null,
                        SubCategories=new List<SubCategory>
                        {
                            new SubCategory
                            {
                                Id="0CE6F3C6-37F1-4244-8641-3F1E9E704C2E",
                                SubCategoryName="Electronic Product",
                                IsActive=true,
                                CreatedDateTime=dateTime,
                                UpdatedDateTime=dateTime,
                                ModifyBy=userId,
                                IP=ipAddress,
                                Products=new List<Product>
                                {
                                    new Product
                                    {
                                        Id="27588397-CD62-48A6-A6E2-C290AAA508EC",
                                        ProductName="Electronic 1",
                                        IsActive=true,
                                        CreatedDateTime=dateTime,
                                        UpdatedDateTime=dateTime,
                                        ModifyBy=userId,
                                        BrandId="E072A4F2-9854-488F-AAD3-F0510824FD02",
                                        IP=ipAddress,
                                        ImageUrl=null,
                                        CategoryId="94661C67-9D3C-45F5-A6DE-50A28B3B1FF6",
                                        Description=null
                                    },
                                    new Product
                                    {
                                        Id="A08C48C5-CA5D-4044-A10F-D380DD9825EC",
                                        ProductName="Electronic 2",
                                        IsActive=true,
                                        CreatedDateTime=dateTime,
                                        UpdatedDateTime=dateTime,
                                        ModifyBy=userId,
                                        BrandId="E072A4F2-9854-488F-AAD3-F0510824FD02",
                                        IP=ipAddress,
                                        ImageUrl=null,
                                        CategoryId="94661C67-9D3C-45F5-A6DE-50A28B3B1FF6",
                                        Description=null
                                    },
                                    new Product
                                    {
                                        Id="7043013F-98E4-42F6-9D9D-BBCCB867466A",
                                        ProductName="Electronic 3",
                                        IsActive=true,
                                        CreatedDateTime=dateTime,
                                        UpdatedDateTime=dateTime,
                                        ModifyBy=userId,
                                        BrandId="E072A4F2-9854-488F-AAD3-F0510824FD02",
                                        IP=ipAddress,
                                        ImageUrl=null,
                                        CategoryId="94661C67-9D3C-45F5-A6DE-50A28B3B1FF6",
                                        Description=null
                                    }
                                }
                            },
                            new SubCategory
                            {
                                Id="A59B9558-B583-4BC0-B7FA-8087869B759E",
                                SubCategoryName="Electronic Services",
                                IsActive=true,
                                CreatedDateTime=dateTime,
                                UpdatedDateTime=dateTime,
                                ModifyBy=userId,
                                IP=ipAddress,
                                Products=new List<Product>
                                {
                                    new Product
                                    {
                                        Id="8FE7F0E8-132A-4A8A-A566-2159C18C8B14",
                                        ProductName="TV service",
                                        IsActive=true,
                                        CreatedDateTime=dateTime,
                                        UpdatedDateTime=dateTime,
                                        ModifyBy=userId,
                                        BrandId="E072A4F2-9854-488F-AAD3-F0510824FD02",
                                        IP=ipAddress,
                                        ImageUrl=null,
                                        CategoryId="94661C67-9D3C-45F5-A6DE-50A28B3B1FF6",
                                        Description=null
                                    },
                                    new Product
                                    {
                                        Id="B1689CE6-8208-4822-9735-96B285156EDC",
                                        ProductName="Mobile Services",
                                        IsActive=true,
                                        CreatedDateTime=dateTime,
                                        UpdatedDateTime=dateTime,
                                        ModifyBy=userId,
                                        BrandId="E072A4F2-9854-488F-AAD3-F0510824FD02",
                                        IP=ipAddress,
                                        ImageUrl=null,
                                        CategoryId="94661C67-9D3C-45F5-A6DE-50A28B3B1FF6",
                                        Description=null
                                    },
                                    new Product
                                    {
                                        Id="BC29FB4A-EE64-47CD-BB60-E5B38E7C96F1",
                                        ProductName="Dish Services",
                                        IsActive=true,
                                        CreatedDateTime=dateTime,
                                        UpdatedDateTime=dateTime,
                                        ModifyBy=userId,
                                        IP=ipAddress,
                                        BrandId="E072A4F2-9854-488F-AAD3-F0510824FD02",
                                        ImageUrl=null,
                                        CategoryId="94661C67-9D3C-45F5-A6DE-50A28B3B1FF6",
                                        Description=null
                                    },
                                    new Product
                                    {
                                        Id="DBA5315C-9210-47BF-A3AC-AB75ECB56F3C",
                                        ProductName="LED Servies",
                                        IsActive=true,
                                        CreatedDateTime=dateTime,
                                        UpdatedDateTime=dateTime,
                                        BrandId="E072A4F2-9854-488F-AAD3-F0510824FD02",
                                        ModifyBy=userId,
                                        IP=ipAddress,
                                        ImageUrl=null,
                                        CategoryId="94661C67-9D3C-45F5-A6DE-50A28B3B1FF6",
                                        Description=null
                                    }
                                }
                            }
                        }
                    }
                };
                context.Categories.AddRange(categories);

                var units = new List<Unit>
                {
                    new Unit
                    {
                         UnitType="Per Kg"
                    },
                    new Unit
                    {
                         UnitType="Per 100gm"
                    },new Unit
                    {
                         UnitType="Per Dozen"
                    },new Unit
                    {
                         UnitType="Per Pice"
                    }
                };
                context.Unit.AddRange(units);

                var orderstatuses = new List<OrderStatus>
                {
                    new OrderStatus
                    {
                        Id=1,
                        StatusName="Visit At Shop"
                    },
                    new OrderStatus
                    {
                        Id=2,
                        StatusName="Under Preparation"
                    },
                    new OrderStatus
                    {
                        Id=3,
                        StatusName="Ready For Delivery"
                    },
                    new OrderStatus
                    {
                        Id=4,
                        StatusName="Cancel"
                    },
                    new OrderStatus
                    {
                        Id=5,
                        StatusName="Delivered"
                    },
                };
                context.OrderStatuses.AddRange(orderstatuses);
                
                context.Brands.AddRange(brands);
                var locations = new List<Location>
                {
                    new Location
                    {
                        Id="9ACF1261-2FF4-4868-AA27-CAB47D79D2D8",
                        CountryId=1,
                        CountryName="India",
                        DistrictId=1,
                        DistrictName="Bokaro",
                        IsActive=true,
                        CreatedDateTime=dateTime,
                        UpdatedDateTime=dateTime,
                        ModifyBy=userId,
                        IP=ipAddress,
                        StateName="Jharkhand",
                        StateId=1
                    },
                    new Location
                    {
                        Id="A4B13207-5F4B-4793-A55E-412EB80E6905",
                        CountryId=1,
                        CountryName="India",
                        DistrictId=1,
                        DistrictName="Dhanbad",
                        IsActive=true,
                        CreatedDateTime=dateTime,
                        UpdatedDateTime=dateTime,
                        ModifyBy=userId,
                        IP=ipAddress,
                        StateName="Jharkhand",
                        StateId=1
                    }
                };
                context.Locations.AddRange(locations);
                
                var shops = new List<Shop>
                { 
                    new Shop
                    {
                        Id="5AA9F351-8D29-498C-A8D2-5ECABFDCE3DC",
                        ShopName="ABC Market",
                        ShopCode="SHOP001",
                        Address1="Chas",
                        Address2="Near ATM bypass",
                        City="Bokaro",
                        CreatedDateTime=dateTime,
                        UpdatedDateTime=dateTime,
                        IP=ipAddress,
                        IsActive=true,
                        IsPriceVisible=true,
                        ModifyBy=userId,
                        LocationId="9ACF1261-2FF4-4868-AA27-CAB47D79D2D8",
                        UserId= userId,
                       // tableName=tableNameShop.TableName
                    },
                    new Shop
                    {
                        Id="5AA9F351-8D29-498C-A8D2-5ECABFDCE3DD",
                        ShopName="XYZ Market Baghmara",
                        ShopCode="SHOP002",
                        Address1="Baghmara",
                        Address2="Near bypass",
                        City="Dhanbad",
                        CreatedDateTime=dateTime,
                        UpdatedDateTime=dateTime,
                        IP=ipAddress,
                        IsActive=true,
                        IsPriceVisible=true,
                        ModifyBy=userId2,
                        LocationId="9ACF1261-2FF4-4868-AA27-CAB47D79D2D8",
                        UserId= userId2,
                        //tableName=tableNameShop.TableName

                    }
                };
                context.Shops.AddRange(shops);
                //context.DynamicTableNames.Add(tableNameShop);
                context.DynamicTableNames.Add(tableNameUser);
                context.SaveChanges();
            }
        }
    }
}
